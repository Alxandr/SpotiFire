using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using SpotiFire.SpotifyLib;
namespace SpotiFire.Server
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    class SpotifireServer : ISpotifireServer
    {
        public static SpotifireServer Instance { get; private set; }
        private BASSPlayer player = null;
        static SpotifireServer()
        {
            Instance = null;
        }

        private string password;
        private ISession spotify;
        private LiveQueue<ITrack> playQueue;
        private Queue<ClientAction> messageQueue;
        private Thread messageThread;
        private AutoResetEvent messageLock;
        public SpotifireServer(string password = null)
        {
            if (Instance != null)
                throw new InvalidOperationException("Can't have 2 servers!");
            Instance = this;

            this.password = password;
            this.spotify = Spotify.CreateSession(Program.applicationKey, Program.cacheLocation, Program.settingsLocation, Program.userAgent);
            this.spotify.SetPrefferedBitrate(sp_bitrate.BITRATE_320k);

            this.spotify.LoginComplete += new SessionEventHandler(spotify_LoginComplete);
            this.spotify.LogoutComplete += new SessionEventHandler(spotify_LogoutComplete);
            this.spotify.MusicDeliver += new MusicDeliveryEventHandler(spotify_MusicDeliver);
            this.spotify.EndOfTrack += new SessionEventHandler(spotify_EndOfTrack);

            this.playQueue = new LiveQueue<ITrack>();

            this.messageQueue = new Queue<ClientAction>();
            this.messageLock = new AutoResetEvent(false);
            this.messageThread = new Thread(new ThreadStart(MessageThread));
            this.messageThread.IsBackground = true;
            this.messageThread.Start();
        }

        void spotify_EndOfTrack(ISession sender, SessionEventArgs e)
        {
            if (playQueue.Count > 0)
            {
                ITrack track = playQueue.Dequeue();
                spotify.PlayerLoad(track);
                spotify.PlayerPlay();
            }
        }

        void spotify_MusicDeliver(ISession sender, MusicDeliveryEventArgs e)
        {
            if (e.Samples.Length > 0)
            {
                if (player == null)
                    player = new BASSPlayer();

                e.ConsumedFrames = player.EnqueueSamples(e.Channels, e.Rate, e.Samples, e.Frames);
            }
            else
            {
                e.ConsumedFrames = 0;
            }
        }

        void spotify_LogoutComplete(ISession sender, SessionEventArgs e)
        {
            //x throw new NotImplementedException();
            //? Hvordan bør dette implementeres?
        }

        void spotify_LoginComplete(ISession sender, SessionEventArgs e)
        {
            if (spotify.ConnectionState == sp_connectionstate.LOGGED_IN)
                MessageClients(client => client.LoginComplete());
            else
                MessageClients(client => client.LoginFailed());
        }

        internal void MessageClients(Action<ISpotifireClient> message, Client client = null)
        {
            ClientAction ca = new ClientAction((c) =>
            {
                message(c.Connection);
            }, client);
            lock (messageQueue)
            {
                messageQueue.Enqueue(ca);
                messageLock.Set();
            }
        }

        void MessageThread()
        {
            Queue<ClientAction> queue = new Queue<ClientAction>();
            while (true)
            {
                messageLock.WaitOne();
                lock (messageQueue)
                {
                    for (int i = 0, l = messageQueue.Count; i < l; i++)
                        queue.Enqueue(messageQueue.Dequeue());
                }
                while (queue.Count > 0)
                {
                    var message = queue.Dequeue();
                    try { message.Execute(); }
                    catch { }
                }
            }
        }

        ~SpotifireServer()
        {
            GC.KeepAlive(spotify);
            spotify.Dispose();
        }

        public bool Authenticate(string password)
        {
            if (this.password != null && password != this.password)
            {
                return false;
            }

            Client c = Client.Current;
            c.Authenticated = true;

            if (spotify.ConnectionState == sp_connectionstate.LOGGED_OUT)
                c.Connection.RequireLogin();

            return true;
        }

        public void Login(string username, string password)
        {
            if (spotify.ConnectionState == sp_connectionstate.LOGGED_OUT)
                spotify.Login(username, password);
        }

        public void Pong()
        {
            Client.UpdateCurrent();
        }


        public Playlist[] GetPlaylists()
        {
            while (!spotify.PlaylistContainer.IsLoaded)
            {
                Thread.Sleep(TimeSpan.FromSeconds(.5));
            }

            return spotify.PlaylistContainer.Playlists.Select(pl => Playlist.Get(pl)).ToArray();
        }

        public Track[] GetPlaylistTracks(Guid id)
        {
            Playlist playlist = Playlist.GetById(id);
            return playlist.playlist.Tracks.Select(t => new Track(t)).ToArray();
        }

        public void PlayPlaylistTrack(Guid playlistId, int position)
        {
            Playlist playlist = Playlist.GetById(playlistId);
            spotify.PlayerLoad(playlist.playlist.Tracks[position]);
            playQueue.Feed = playlist.playlist.Tracks.Cast<ITrack>();
            playQueue.Index = position;
            spotify.PlayerPlay();
        }

    }
}

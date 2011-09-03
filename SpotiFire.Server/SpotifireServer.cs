using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.Threading;
using SpotiFire.SpotifyLib;
using c = System.ServiceModel.Channels;
namespace SpotiFire.Server
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    class SpotifireServer : ISpotifireServer, IErrorHandler
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
        private AutoResetEvent loginComplete = new AutoResetEvent(false);
        private readonly object canLoginLock = new object();
        private Playlist playlist = null;
        private ManualResetEvent waitHandle = new ManualResetEvent(false);

        private DateTime trackStartTime = DateTime.Now;
        private DateTime trackPauseTime = DateTime.MinValue;

        private bool playing = false;

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

            this.spotify.StartPlayback += new SessionEventHandler(spotify_StartPlayback);
            this.spotify.StopPlayback += new SessionEventHandler(spotify_StopPlayback);

            this.playQueue = new LiveQueue<ITrack>();

            this.messageQueue = new Queue<ClientAction>();
            this.messageLock = new AutoResetEvent(false);
            this.messageThread = new Thread(new ThreadStart(MessageThread));
            this.messageThread.IsBackground = true;
            this.messageThread.Start();

            player = new BASSPlayer();
        }

        void spotify_StopPlayback(ISession sender, SessionEventArgs e)
        {
            if (trackPauseTime == DateTime.MinValue)
            {
                trackPauseTime = DateTime.Now;
                playing = false;
                MessageClients(c => c.PlaybackEnded());
            }
        }

        void spotify_StartPlayback(ISession sender, SessionEventArgs e)
        {
            if (trackPauseTime != DateTime.MinValue)
            {
                trackStartTime = trackStartTime + DateTime.Now.Subtract(trackPauseTime);
                trackPauseTime = DateTime.MinValue;
                playing = true;
                MessageClients(c => c.PlaybackStarted());
            }
        }

        public void Run()
        {
            waitHandle.WaitOne();
        }

        void spotify_SongStarted(ISession sender, EventArgs e)
        {
            playing = true;
            ITrack track = playQueue.Current;
            IPlaylistTrack pt = track as IPlaylistTrack;
            if (pt != null)
            {
                int index = playlist.playlist.Tracks.IndexOf(pt);
                if (index != -1)
                {
                    MessageClients(c => c.SongStarted(new Track(track), playlist.Id, index));
                    return;
                }
            }

            MessageClients(c => c.SongStarted(new Track(track), Guid.Empty, 0));
        }

        void spotify_EndOfTrack(ISession sender, SessionEventArgs e)
        {
            if (playQueue.Count > 0)
            {
                ITrack track = playQueue.Dequeue();
                spotify.PlayerLoad(track);
                spotify.PlayerPlay();
                spotify_SongStarted(spotify, new EventArgs());
            }
            else
            {
                playQueue.Dequeue(true);
            }
        }

        void spotify_MusicDeliver(ISession sender, MusicDeliveryEventArgs e)
        {
            if (e.Samples.Length > 0)
            {
                e.ConsumedFrames = player.EnqueueSamples(e.Channels, e.Rate, e.Samples, e.Frames);
            }
            else
            {
                e.ConsumedFrames = 0;
            }
        }

        void spotify_LogoutComplete(ISession sender, SessionEventArgs e)
        {
            // Message about logout.
        }

        void spotify_LoginComplete(ISession sender, SessionEventArgs e)
        {
            loginComplete.Set();
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
                    try { message.Execute(); } // Should be fail-safe, but don't want thread to exit just in case.
                    catch { }
                }
            }
        }

        ~SpotifireServer()
        {
            GC.KeepAlive(spotify);
            spotify.Dispose();
        }

        public AuthenticationStatus Authenticate(string password)
        {
            if (this.password != null && password != this.password)
            {
                return AuthenticationStatus.Bad;
            }

            Client c = Client.Current;
            c.Authenticated = true;

            if (spotify.ConnectionState == sp_connectionstate.LOGGED_OUT)
                return AuthenticationStatus.RequireLogin;

            return AuthenticationStatus.Ok;
        }

        public bool Login(string username, string password)
        {
            lock (canLoginLock)
            {
                if (spotify.ConnectionState == sp_connectionstate.LOGGED_OUT)
                {
                    spotify.Login(username, password, false);
                    loginComplete.WaitOne();
                    return spotify.ConnectionState == sp_connectionstate.LOGGED_IN;
                }
                else
                {
                    return true;
                }
            }
        }

        public void Pong()
        {
            Client.UpdateCurrent();
        }


        public IEnumerable<Playlist> GetPlaylists()
        {
            while (!spotify.PlaylistContainer.IsLoaded)
            {
                Thread.Sleep(TimeSpan.FromSeconds(.5));
            }

            yield return Playlist.Get(spotify.Starred);
            foreach (var p in spotify.PlaylistContainer.Playlists)
                yield return Playlist.Get(p);
        }

        public IEnumerable<Track> GetPlaylistTracks(Guid id)
        {
            Playlist playlist = Playlist.GetById(id);
            return playlist.playlist.Tracks.Select(t => new Track(t)).ToArray();
        }

        public void PlayPlaylistTrack(Guid playlistId, int position)
        {
            Playlist playlist = Playlist.GetById(playlistId);
            this.playlist = playlist;
            if (position != -1)
                spotify.PlayerLoad(playlist.playlist.Tracks[position]);
            playQueue.Feed = playlist.playlist.Tracks.Cast<ITrack>();
            if (position == -1)
                spotify.PlayerLoad(playQueue.Dequeue());
            if (position != -1)
                playQueue.Index = position;
            if (position != -1)
                playQueue.Current = playlist.playlist.Tracks[position];
            spotify.PlayerPlay();
            spotify_SongStarted(spotify, new EventArgs());
        }



        public void SetShuffle(bool shuffle)
        {
            playQueue.Shuffle = shuffle;
        }

        public void SetRepeat(bool repeat)
        {
            playQueue.Repeat = repeat;
        }

        public void Exit()
        {
            waitHandle.Set();
        }

        public void SetVolume(int volume)
        {
            volume = Math.Max(Math.Max(volume, 0), 100);
            player.Volume = (volume / 100);
        }

        public int GetVolume()
        {
            return (int)(player.Volume * 100);
        }

        public void PlayPause()
        {
            playing = !playing;
            if (!playing)
                spotify.PlayerPause();
            else
                spotify.PlayerPlay();
        }

        public bool HandleError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void ProvideFault(Exception error, c.MessageVersion version, ref c.Message fault)
        {
            if (error is FaultException)
                return;
        }

        public SpotifyStatus GetStatus()
        {
            return new SpotifyStatus
            {
                IsPlaying = playing,
                CanGoBack = false,
                CanGoNext = false,
                CanStartPlayback = playQueue.Current != null,
                CurrentTrack = playQueue.Current != null ? new Track(playQueue.Current) : null,
                LengthPlayed = playQueue.Current != null ? (playing ? DateTime.Now.Subtract(trackStartTime) : trackPauseTime.Subtract(trackStartTime)) : TimeSpan.Zero,
                Repeat = playQueue.Repeat,
                Shuffle = playQueue.Shuffle,
                Volume = GetVolume()
            };
        }
    }
}

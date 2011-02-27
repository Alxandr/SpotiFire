using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using SpotiFire.SpotiClient.ServiceReference;

namespace SpotiFire.SpotiClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ServiceReference.SpotifyCallback
    {
        private ServiceReference.SpotifyClient client;

        public App()
            : base()
        {
            client = new ServiceReference.SpotifyClient(new InstanceContext(this));
            AuthenticationStatus ok = AuthenticationStatus.Bad;
            MainWindow = new MainWindow(this);
            try { ok = client.Authenticate(""); }
            catch { throw; }
            if (ok == AuthenticationStatus.Bad)
                Shutdown();
            else if (ok == AuthenticationStatus.RequireLogin)
            {
                bool loggedIn = false;
                while (!loggedIn)
                {
                    SpotifyLogin login = new SpotifyLogin();
                    login.ShowDialog();
                    if (login.Username.Text != "")
                        loggedIn = client.Login(login.Username.Text, login.Password.Password);
                }
            }
            MainWindow = new MainWindow(this);
            MainWindow.Show();
            MainWindow.Closed += new EventHandler(MainWindow_Closed);
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            this.Shutdown();
        }

        public void Ping()
        {
            client.Pong();
        }

        public PlaylistTreeItem[] GetPlaylists()
        {
            return MakePlaylistTree(client.GetPlaylists()).ToArray();
        }

        private IEnumerable<PlaylistTreeItem> MakePlaylistTree(Playlist[] playlists)
        {
            for (int i = 0; i < playlists.Length; i++)
            {
                if (playlists[i].Type == PlaylistType.Playlist)
                    yield return new PlaylistTreeItem(playlists[i]);

                else if (playlists[i].Type == PlaylistType.FolderStart)
                {
                    Playlist p = playlists[i];
                    int depth = 1;
                    IList<Playlist> intermedium = new List<Playlist>();
                    for (i++; i < playlists.Length; i++)
                    {
                        if (playlists[i].Type == PlaylistType.Playlist)
                            intermedium.Add(playlists[i]);
                        else if (playlists[i].Type == PlaylistType.FolderStart)
                        {
                            intermedium.Add(playlists[i]);
                            depth++;
                        }
                        else if (playlists[i].Type == PlaylistType.FolderEnd)
                        {
                            if (--depth == 0)
                            {
                                yield return new PlaylistTreeItem(p, MakePlaylistTree(intermedium.ToArray()).ToArray());
                                break;
                            }
                            else
                            {
                                intermedium.Add(playlists[i]);
                            }
                        }
                    }
                }
            }
            yield break;
        }

        public Track[] GetPlaylistTracks(Guid guid)
        {
            return client.GetPlaylistTracks(guid);
        }

        public void PlayPlaylistTrack(Guid guid, int index)
        {
            client.PlayPlaylistTrack(guid, index);
        }

        internal void SetRandom(bool p)
        {
            client.SetRandom(p);
        }

        internal void SetRepeat(bool p)
        {
            client.SetRepeat(p);
        }

        public void PlaybackStarted(Track track, Guid containerId, int index)
        {
            if (OnPlaybackStart != null)
                OnPlaybackStart(this, new PlaybackEventArgs(track, containerId, index));
        }

        public event EventHandler<PlaybackEventArgs> OnPlaybackStart;
    }
}

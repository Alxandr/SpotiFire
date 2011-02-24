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
            bool ok = false;
            try { ok = client.Authenticate("tester"); }
            catch { throw; }
            if (!ok)
                Shutdown();
            else
                MainWindow = new MainWindow(this);
        }

        public void LoginComplete()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                MainWindow.Show();
            }));
        }

        public void LoginFailed()
        {
            RequireLogin();
        }

        public void RequireLogin()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                SpotifyLogin login = new SpotifyLogin();
                login.ShowDialog();
                string username = login.Username.Text;
                if (!string.IsNullOrWhiteSpace(username))
                    client.Login(login.Username.Text, login.Password.Password);
            }));
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
    }
}

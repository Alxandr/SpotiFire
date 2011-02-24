using System.Windows;
using SpotiFire.SpotiClient.ServiceReference;

namespace SpotiFire.SpotiClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        App app;
        Playlist playlist = null;
        public MainWindow(App app)
        {
            this.app = app;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PlaylistTreeItem[] playlists = app.GetPlaylists();
            SpotifyTree.ItemsSource = playlists;
        }

        private void SpotifyTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            PlaylistTreeItem playlist = e.NewValue as PlaylistTreeItem;
            if (playlist.Type == ServiceReference.PlaylistType.Playlist)
            {
                Track[] tracks = app.GetPlaylistTracks(playlist.Id);
                this.playlist = playlist;

                SpotifySongs.ItemsSource = tracks;
            }
        }

        private void SpotifySongs_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            int index = SpotifySongs.SelectedIndex;
            app.PlayPlaylistTrack(playlist.Id, index);
        }
    }
}

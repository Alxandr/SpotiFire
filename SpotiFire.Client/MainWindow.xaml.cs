using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
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
        DispatcherTimer timer;
        DateTime startTime = DateTime.Now;
        TimeSpan songLength = TimeSpan.Zero;
        public MainWindow(App app)
        {
            this.app = app;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PlaylistTreeItem[] playlists = app.GetPlaylists();
            SpotifyTree.ItemsSource = playlists;
            app.OnPlaybackStart += new System.EventHandler<PlaybackEventArgs>(app_OnPlaybackStart);
            timer = new DispatcherTimer(TimeSpan.FromSeconds(0.1), DispatcherPriority.DataBind, new EventHandler(timer_Tick), Dispatcher);
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs args)
        {
            TimeSpan current = DateTime.Now.Subtract(startTime);
            int value = (int)Math.Floor(current.TotalSeconds);
            if (ProgressSlider.Value < ProgressSlider.Maximum)
            {
                ProgressSlider.Value = Math.Min(ProgressSlider.Maximum, value);
                if (current < songLength)
                    SongProgress.Text = current.ToString(@"mm\:ss");
                else
                    SongProgress.Text = current.ToString(@"mm\:ss");
            }
        }

        void app_OnPlaybackStart(object sender, PlaybackEventArgs e)
        {
            songLength = e.Track.Length;
            startTime = DateTime.Now;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ProgressSlider.Maximum = songLength.TotalSeconds;
                SongTotalLength.Text = songLength.ToString(@"mm\:ss");
            }));
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
            var item = sender as ListViewItem;
            if (item != null)
            {
                int index = SpotifySongs.Items.IndexOf(item.Content);
                if (index != -1)
                    app.PlayPlaylistTrack(playlist.Id, index);
            }
        }

        private void RandomButton_Checked(object sender, RoutedEventArgs e)
        {
            app.SetRandom(RandomButton.IsChecked.Value);
        }

        private void RepeatButton_Checked(object sender, RoutedEventArgs e)
        {
            app.SetRepeat(RepeatButton.IsChecked.Value);
        }

        private void SpotifyTree_ItemDoubleClicked(object sender, RoutedEventArgs e)
        {
            var item = sender as TreeViewItem;
            if (item != null)
            {
                PlaylistTreeItem pti = item.DataContext as PlaylistTreeItem;
                if (pti != null)
                {
                    app.PlayPlaylistTrack(pti.Id, -1);
                }
            }
        }

    }
}

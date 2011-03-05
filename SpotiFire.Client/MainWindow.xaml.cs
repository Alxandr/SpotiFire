using System;
using System.Windows;
using System.Windows.Threading;
using SpotiFire.SpotiClient.ViewModels;

namespace SpotiFire.SpotiClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;

        SpotifyViewModel model;

        public MainWindow(SpotifyViewModel model)
        {
            this.model = model;
            InitializeComponent();
            DataContext = model;
            model.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(model_PropertyChanged);
        }

        void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Playlists")
            {
                //SpotifyTree.ItemsSource = model.Playlists;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //app.OnPlaybackStart += new System.EventHandler<PlaybackEventArgs>(app_OnPlaybackStart);
            timer = new DispatcherTimer(TimeSpan.FromSeconds(0.1), DispatcherPriority.DataBind, new EventHandler(timer_Tick), Dispatcher);
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs args)
        {
            bool can = model.CanGoBack;
            //? Query for model playback length
        }

        private void SpotifyTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //PlaylistTreeItem playlist = e.NewValue as PlaylistTreeItem;
            //if (playlist.Type == ServiceReference.PlaylistType.Playlist)
            //{
            //    Track[] tracks = app.GetPlaylistTracks(playlist.Id);
            //    this.playlist = playlist;

            //    SpotifySongs.ItemsSource = tracks;
            //}
        }

        private void SpotifySongs_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //var item = sender as ListViewItem;
            //if (item != null)
            //{
            //    int index = SpotifySongs.Items.IndexOf(item.Content);
            //    if (index != -1)
            //        app.PlayPlaylistTrack(playlist.Id, index);
            //}
        }

        private void ShuffleButton_Checked(object sender, RoutedEventArgs e)
        {
            //app.SetRandom(model.Shuffle);
        }

        private void RepeatButton_Checked(object sender, RoutedEventArgs e)
        {
            //app.SetRepeat(model.Repeat);
        }

        private void SpotifyTree_ItemDoubleClicked(object sender, RoutedEventArgs e)
        {
            //var item = sender as TreeViewItem;
            //if (item != null)
            //{
            //    PlaylistTreeItem pti = item.DataContext as PlaylistTreeItem;
            //    if (pti != null)
            //    {
            //        app.PlayPlaylistTrack(pti.Id, -1);
            //    }
            //}
        }

        private void ExitMenuButton_Click(object sender, RoutedEventArgs e)
        {
            //app.CloseAll();
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //app.Volume = (int)e.NewValue;
        }

        private void Artist_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            //app.PlayPause();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpotiFire.WpfTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Session session;
        
        static BASSPlayer player = new BASSPlayer();

        private PlaylistContainer pc;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SetupSession()
        {
            session.MusicDelivered += session_MusicDeliverd;
        }

        void session_MusicDeliverd(Session sender, MusicDeliveryEventArgs e)
        {
            e.ConsumedFrames = player.EnqueueSamples(e.Channels, e.Rate, e.Samples, e.Frames);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            session = await Spotify.Task;
            SetupSession();
            while (true)
            {
                var login = new Login(session);
                var result = login.ShowDialog();
                if (result.HasValue && result.Value)
                    break;
                else if (result.HasValue)
                {
                    Application.Current.Shutdown();
                    return;
                }
            }

            pc = await session.PlaylistContainer;
            List<Playlist> playlists = new List<Playlist>(pc.Playlists.Count);
            foreach (var playlist in pc.Playlists)
            {
                playlists.Add(await playlist);
            }

            Playlists.ItemsSource = playlists;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            // perform search
            var term = Search.Text;
            var search = await session.SearchTracks(term, 0, 100);

            List<Track> tracks = new List<Track>();
            foreach (var t in search.Tracks)
                tracks.Add(await t);

            Songs.ItemsSource = tracks;
        }

        private async void Playlists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                var itm = e.AddedItems[0] as Playlist;
                if (itm == null) return;

                await itm;
                List<Track> tracks = new List<Track>();
                foreach (var t in itm.Tracks)
                    tracks.Add(await t);

                Songs.ItemsSource = tracks;
            }
        }

        private async void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock tb = sender as TextBlock;
            if (tb == null) return;
            // click song
            if (e.ClickCount == 2) // double
            {
                var track = tb.DataContext as Track;
                if (track == null)
                    return;

                await track;
                session.PlayerUnload();
                session.PlayerLoad(track);
                session.PlayerPlay();
            }
        }
    }
}

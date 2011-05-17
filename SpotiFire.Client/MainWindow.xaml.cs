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
            //if (e.PropertyName == "CurrentView")
            //    ViewSpace.Child = model.CurrentView.GetView();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //app.OnPlaybackStart += new System.EventHandler<PlaybackEventArgs>(app_OnPlaybackStart);
            timer = new DispatcherTimer(TimeSpan.FromSeconds(0.1), DispatcherPriority.DataBind, new EventHandler(timer_Tick), Dispatcher);
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs args)
        {
            model.UpdateTrackTime();
        }

    }
}

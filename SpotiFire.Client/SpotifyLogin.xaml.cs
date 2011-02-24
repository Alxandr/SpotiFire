using System.Windows;

namespace SpotiFire.SpotiClient
{
    /// <summary>
    /// Interaction logic for SpotifyLogin.xaml
    /// </summary>
    public partial class SpotifyLogin : Window
    {
        public SpotifyLogin()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Username.Focus();
        }
    }
}

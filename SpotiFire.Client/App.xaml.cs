using System;
using System.ServiceModel;
using System.Windows;
using SpotiFire.SpotiClient.ServiceReference;
using SpotiFire.SpotiClient.ViewModels;

namespace SpotiFire.SpotiClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceReference.SpotifyClient client;
        private SpotifyViewModel model;

        public App()
            : base()
        {
            model = SpotifyViewModel.Instance;
            client = new ServiceReference.SpotifyClient(new InstanceContext(model));
            MainWindow = new MainWindow(model);
            AuthenticationStatus ok = AuthenticationStatus.Bad;
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
            model.Client = client;
            MainWindow.Show();
            MainWindow.Closed += new EventHandler(MainWindow_Closed);
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            this.Shutdown();
        }
    }
}

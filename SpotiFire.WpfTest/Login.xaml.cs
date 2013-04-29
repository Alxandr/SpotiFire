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
using System.Windows.Shapes;

namespace SpotiFire.WpfTest
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        Session _session;
        public Login(Session session)
        {
            _session = session;
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var result = await _session.Login(Username.Text, Password.Password, false);
            if (result == Error.OK)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Bad login");
                Password.Password = "";
            }
        }
    }
}

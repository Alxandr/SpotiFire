using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpotiFire.WinFormsTest
{
    public partial class Login : Form
    {
        private SpotifyLib.ISession session;
        private TaskCompletionSource<bool> _loginWait = new TaskCompletionSource<bool>();

        public Login(SpotifyLib.ISession session)
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;
            // TODO: Complete member initialization
            this.session = session;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var success = await session.Login(textBox1.Text, maskedTextBox1.Text, false);
            if (success)
            {
                DialogResult = DialogResult.OK;
                _loginWait.SetResult(true);
                _loginWait = new TaskCompletionSource<bool>();
                Close();
            }
            else
            {
                MessageBox.Show("Bad login");
                maskedTextBox1.Text = "";
            }
        }

        public Task<bool> DoLogin()
        {
            return _loginWait.Task;
        }
    }
}

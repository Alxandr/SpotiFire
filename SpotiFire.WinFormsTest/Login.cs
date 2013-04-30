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
        private Session session;
        private TaskCompletionSource<bool> _loginWait = new TaskCompletionSource<bool>();

        public Login(Session session)
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;
            // TODO: Complete member initialization
            this.session = session;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var result = await session.Login(textBox1.Text, maskedTextBox1.Text, false);
            if (result == Error.OK)
            {
                DialogResult = DialogResult.OK;
                _loginWait.SetResult(true);
                _loginWait = new TaskCompletionSource<bool>();
                Close();
            }
            else
            {
                MessageBox.Show(result.Message(), "An error occurred while logging in.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                maskedTextBox1.Text = "";
            }
        }

        public Task<bool> DoLogin()
        {
            return _loginWait.Task;
        }
    }
}

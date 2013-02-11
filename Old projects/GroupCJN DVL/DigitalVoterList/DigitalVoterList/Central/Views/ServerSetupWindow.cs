// -----------------------------------------------------------------------
// <copyright file="ServerSetupWindow.cs" company="DVL">
// <author>Jan Meier</author>
// </copyright>
// -----------------------------------------------------------------------


namespace Central.Central.Views
{
    using System.Windows.Forms;

    public partial class ServerSetupWindow : Form
    {
        public ServerSetupWindow()
        {
            this.InitializeComponent();
        }

        private void Form1_Closed()
        {
            Application.Exit();
        }

        public string Address
        {
            get
            {
                return this.adressBox.Text;
            }
        }

        public string Port
        {
            get
            {
                return this.portBox.Text;
            }
        }

        public string User
        {
            get
            {
                return this.userBox.Text;
            }
        }

        public string Pw
        {
            get
            {
                return this.passwordBox.Text;
            }
        }

        public void ShowMessageBox(string text)
        {
            MessageBox.Show(this, text);
        }
    }
}

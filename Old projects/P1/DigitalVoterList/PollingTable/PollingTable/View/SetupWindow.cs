
// -----------------------------------------------------------------------
// <copyright file="Model.cs" company="">
// Author: Claes Martinsen.
// </copyright>
// -----------------------------------------------------------------------

namespace PollingTable.PollingTable.View
{
    using System.Windows.Forms;

    /// <summary>
    /// Window that lets the user type in table number, target connection and password.
    /// </summary>
    public partial class SetupWindow : Form
    {
        
        public SetupWindow(SetupInfo si)
        {
            InitializeComponent();

            ipTextBox.Text = si.Ip;
            tableBox.Text = si.TableNo.ToString();
        }

        public SetupWindow()
        {
            this.InitializeComponent();
        }

        private void Form1_Closed()
        {
            Application.Exit();
        }
    }
}

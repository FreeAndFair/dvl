using System.Windows.Forms;

namespace DVLTerminal.GUI
{
    /// <summary>
    /// The window that is displayed when a vote is being processed,
    /// that is, when the user notifies the other PC's on the network
    /// that 'Person-A' would like to vote. 
    /// </summary>
    public partial class PleaseWait : Form
    {
        public PleaseWait()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Prevents the window from being closed. This is done automatically
        /// eventually.
        /// </summary>
        private void PleaseWait_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}

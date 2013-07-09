using System;
using System.Windows.Forms;

namespace DVLTerminal.GUI
{
    /// <summary>
    /// A window to show that a ballot can be handed out
    /// </summary>
    public partial class AcceptBallotHandout : Form
    {
        public AcceptBallotHandout()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Hides the window when the "Continue" button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void continuebutton_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}

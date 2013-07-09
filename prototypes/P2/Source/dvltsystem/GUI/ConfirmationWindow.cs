using System;
using System.Windows.Forms;

namespace DVLTerminal.GUI
{
    /// <summary>
    /// The confirmation window that is displayed to make sure that all
    /// the PC's that should be connected to the network, are connected
    /// and are ready to start voting.
    /// </summary>
    public partial class ConfirmationWindow : Form
    {
        public ConfirmationWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles whether or not the Event WindowClosed was called with
        /// true or false as parameter.
        /// </summary>
        /// <param name="result"></param>
        public delegate void WindowClosedHandler(bool result);

        public event WindowClosedHandler WindowClosed;

        /// <summary>
        /// Invokes the event WindowClosed with true as parameter
        /// and hides the window.
        /// </summary>
        private void OKButton_Click(object sender, EventArgs e)
        {
            if (WindowClosed != null)
            {
                WindowClosed(true);
            }
            Close();
        }

        /// <summary>
        /// Invokes the event WindowClosed with false as parameter
        /// and hides the window.
        /// </summary>
        private void NoButton_Click(object sender, EventArgs e)
        {
            if (WindowClosed != null)
            {
                WindowClosed(false);
            }
            Close();
        }

        private void ConfirmationWindow_Load(object sender, EventArgs e)
        {

        }
    }
}

using System;
using System.Windows.Forms;

namespace DVLTerminal.GUI
{
    /// <summary>
    /// A windows asking the user for confirmation that they wanted to end the election on this machine.
    /// Uses a generated number for confirmation
    /// </summary>
    public partial class EndElectionWindow : Form
    {
        /// <summary>
        /// The generated confirmation number
        /// </summary>
        private readonly int confirmationNumber;

        /// <summary>
        /// Create a new EndElection window and generate a new confirmation number
        /// </summary>
        public EndElectionWindow()
        {
            InitializeComponent();
            confirmationNumber = new Random().Next(10000, 99999);
            label2.Text = "No.: " + confirmationNumber;
        }

        /// <summary>
        /// Handles a click to the cancel button by hiding the window
        /// </summary>
        private void cancelbutton_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            Hide();
        }

        /// <summary>
        /// Handles a click to the confirm button by validation the confirmation number
        /// If the confirmation number is correct the program is terminated
        /// </summary>
        private void confirmbutton_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0) return;
            try
            {
                int enteredNum = Convert.ToInt32(textBox1.Text);
                if (enteredNum.Equals(confirmationNumber))
                    Environment.Exit(0);
                else
                    MessageBox.Show("Wrong number entered.", "Incorrect code", MessageBoxButtons.OK,MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            catch (FormatException)
            {
                MessageBox.Show("The code can only contain numbers", "Invalid characters", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

    }
}

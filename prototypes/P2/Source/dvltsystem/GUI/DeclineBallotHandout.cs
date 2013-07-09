using System;
using System.Windows.Forms;
using DVLTerminal.Local;

namespace DVLTerminal.GUI
{
    /// <summary>
    /// A window to tell the user that the ballot should not be handed out, and why
    /// </summary>
    public partial class DeclineBallotHandout : Form
    {
        private readonly VoteResult voteResult;
        public DeclineBallotHandout()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Creates a window with a matching notice message depending on
        /// the DatabaseResult type.
        /// </summary>
        /// <param name="voteResult">The reason that the ballot was not handed out.</param>
        public DeclineBallotHandout(VoteResult voteResult)
        {
            this.voteResult = voteResult;
            InitializeComponent();
        }

        /// <summary>
        /// Hides the window when the "Continue" button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void continueButton_Click(object sender, EventArgs e)
        {
            Hide();
        }

        /// <summary>
        /// The method that loads the window, with a text explaining what went wrong
        /// This should be defined as a VoteResult in the voteResult field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeclineBallotHandout_Load(object sender, EventArgs e)
        {
            switch (voteResult)
            {
                case VoteResult.CPRCheckFailed:
                    label1.Text = "The last four digits of the CPR number" + Environment.NewLine + "were not correct.";
                    break;
                case VoteResult.HasVoted:
                    label1.Text = "Warning: the voting card has already" + Environment.NewLine + "been used for voting!";
                    break;
                case VoteResult.NotInDatabase:
                    label1.Text = "The entered voting number cannot" + Environment.NewLine + "be found." + Environment.NewLine + "Please try again or check voting place";
                    break;
            }
        }
    }
}

using System;
using System.Windows.Forms;
using DVLTerminal.Local;
using DVLTerminal.Utilities;

namespace DVLTerminal.GUI
{
    /// <summary>
    /// The main windows shown to the user under normal execution.
    /// Allows registration of votes.
    /// </summary>
    partial class RegisterVoterWindow : Form
    {
        /// <summary>
        /// All the windows that are used, are initialized
        /// </summary>
        readonly PleaseWait _pleaseWait = new PleaseWait();
        readonly DeclineBallotHandout _decBalHanCPRCheckFailed = new DeclineBallotHandout(VoteResult.CPRCheckFailed);
        readonly DeclineBallotHandout _decBalHanHasVoted = new DeclineBallotHandout(VoteResult.HasVoted);
        readonly DeclineBallotHandout _decBalHanNotInDatabase = new DeclineBallotHandout(VoteResult.NotInDatabase);
        readonly AcceptBallotHandout _acceptBallotHandout = new AcceptBallotHandout();
        readonly EndElectionWindow _endElectionWindow = new EndElectionWindow();
        readonly LocalVoteListener localVoteListener;

        public RegisterVoterWindow(LocalVoteListener lvl)
        {
            InitializeComponent();
            localVoteListener = lvl;
        }

        /// <summary>
        /// Handle a press to the submit button
        /// This creates a new vote in LocalVoteListener and shows the "Please wait" window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmitVoteButtonClick(object sender, EventArgs e)
        {
            if (textBoxVotingNum.Text.Length == 0) return;
            try
            {
                ulong votingNumber = Convert.ToUInt64(textBoxVotingNum.Text);
                ushort cprNumber = Convert.ToUInt16(textBoxCPRNum.Text);

                localVoteListener.Vote = new Vote(votingNumber, cprNumber, localVoteListener.Peers);
                _pleaseWait.Show();
                voteUpdatedTimer.Start();
            }
            catch (FormatException)
            {
                MessageBox.Show("The input data contained some illegal characters. Only positive integers are allowed.", "Invalid characters", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        /// <summary>
        /// Keeps track on whether the vote has been finished or not every time timer ticks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void voteUpdatedTimerTick(object sender, EventArgs e)
        {
            if (localVoteListener.Vote.VoteState != VoteState.Done)
                return;

            _pleaseWait.Hide();
            voteUpdatedTimer.Stop();

            switch (localVoteListener.Vote.VoteResult)
            {
                case VoteResult.VoteOk:
                    _acceptBallotHandout.Show();
                    textBoxCPRNum.Clear();
                    textBoxVotingNum.Clear();
                    textBoxVotingNum.Focus();
                    _acceptBallotHandout.Focus();
                    break;
                case VoteResult.CPRCheckFailed:
                    _decBalHanCPRCheckFailed.Show();
                    break;
                case VoteResult.NotInDatabase:
                    _decBalHanNotInDatabase.Show();
                    break;
                case VoteResult.HasVoted:
                    _decBalHanHasVoted.Show();
                    break;
            }
        }

        /// <summary>
        /// Ensures that the window cannot be closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegisterVoterWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        /// <summary>
        /// Shows the EndElectionWindow so the user can confirm they really want to exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEndElection_Click(object sender, EventArgs e)
        {
            _endElectionWindow.Show();
        }

        /// <summary>
        /// Starts the process for showing how big a percentage has voted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegisterVoterWindow_Load(object sender, EventArgs e)
        {   
            voteProgressBar.Maximum = (int)Database.GetInstance.GetTotalVotersCount(); //Safe because a votingplace never will have more than 2 bil. voters.
            getVotersCount.Start();
        }

        /// <summary>
        /// Makes sure that the interface is updated with the newest information on how many has voted, and how many are connected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void getVotersCount_Tick(object sender, EventArgs e)
        {
            connectedComputersLabel.Text = String.Format("Connected to: {0} PC(s)", localVoteListener.Peers.Count);

            var votingCounter = Database.GetInstance.GetHasVotedCount();
            voteProgressBar.Value = (int)votingCounter; //Safe because a votingplace never will have more than 2 bil. voters.
            labelCountVoted.Text = String.Format("Currently {0} {1} has voted.", votingCounter, (votingCounter == 1) ? "person" : "people");

            Refresh();
        }

        /// <summary>
        /// Checks if a keystroke is either 0 - 9, backspace, delete ,left or right arrowbutton
        /// </summary>
        /// <param name="e"></param>
        /// <returns>Bool indicating if the keystroke was one of the required</returns>
        private static bool ValidKey(KeyEventArgs e)
        {
            return ((e.KeyData >= Keys.D0 && e.KeyData <= Keys.D9) || e.KeyData == Keys.Back || e.KeyData == Keys.Delete || e.KeyData == Keys.Left || e.KeyData == Keys.Right);
        }

        /// <summary>
        /// Suprpresses the keystroke in the textbox votingNum if it's not allowed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxVotingNum_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = !ValidKey(e);
        }

        /// <summary>
        /// Suprpresses the keystroke in the textbox CPRNum if it's not allowed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxCPRNum_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = !ValidKey(e);
        }
    }
}
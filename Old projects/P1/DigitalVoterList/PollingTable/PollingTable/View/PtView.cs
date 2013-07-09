// -----------------------------------------------------------------------
// <copyright file="PtView.cs" company="">
// Author: Claes Martinsen.
// </copyright>
// -----------------------------------------------------------------------

namespace PollingTable.PollingTable.View
{
    using DBComm.DBComm.DO;

    using global::PollingTable.PollingTable;
    using global::PollingTable.PollingTable.Log;

    /// <summary>
    /// The view is responsible for providing GUI elements for the user 
    /// and for showing the state of the model.
    /// </summary>
    public class PtView
    {
        private Model model;

        private ScannerWindow scannerWindow;

        private SetupWindow setupWindow; //window used for polling table setup 
        private UnRegVW unregWindow; //Window used for unregistering
        private WarningVW warningWindow; //window used when the voter has already been registered.
        private NormVW normalVoterWindow; //Window that shows the voter's information and where the user can register the voter.

        public delegate void VoterShownHandler();
        public delegate void UnlockHandler();
        public delegate void UnregisterHandler(string admpass);

        public event VoterShownHandler VoterShown; //Notify when the user clicks in the register button.
        public event UnlockHandler Unlock; //Notify when the user clicks the unlock button.
        public event UnregisterHandler Unregister; //Notify when the user clicks the unregister.

        /// <summary>
        /// Initializes the setup and scanner windows and listens for changes in the model (observer pattern).
        /// </summary>
        /// <param name="model"></param>
        public PtView(Model model)
        {
            this.model = model;
            scannerWindow = new ScannerWindow();
            setupWindow = new SetupWindow();

            //When the user clicks on the lock to open the unregister window
            scannerWindow.LockBtn.Click += (o, eA) => this.OpenLogWindow();

            model.CurrentVoterChanged += this.ShowSpecificVoter;
            model.SetupInfoChanged += this.UpdateSetupWindow;
            model.ConnectionError += this.ConnectionLostMsg;

            this.Unlock += this.OpenUnregWindow;
        }

        /// <summary>
        /// Updates 
        /// </summary>
        /// <param name="voter"></param>
        public void ShowSpecificVoter(VoterDO voter)
        {
            //If the voter is null, it doesn't exists in the database
            if (voter == null)
            {
                return;
            }

            //Open an ordinary voter window if the voter has not voted yet.
            if (voter.Voted == false) this.OpenNormalWindow(voter);

            //Open a voter window with warning message indicating that the voter has alredy voted.
            if (voter.Voted == true) this.OpenWarningWindow(voter);
        }

        /// <summary>
        /// Opens the warning window where the user is warned that the voter has already vote. 
        /// </summary>
        /// <param name="voter">The voter to be shown</param>
        public void OpenWarningWindow(VoterDO voter)
        {
            {
                warningWindow = new WarningVW(voter);
                this.warningWindow.UnlockButton.Click += (o, eA) => this.Unlock();
                this.warningWindow.UnlockButton.Click += (o, eA) => this.warningWindow.Close();
                this.warningWindow.ShowDialog();
            }
        }

        /// <summary>
        /// Opens the normal voter window where the user can register the voter as registered. 
        /// </summary>
        /// <param name="voter">The voter to be shown</param>
        public void OpenNormalWindow(VoterDO voter)
        {
            this.normalVoterWindow = new NormVW(voter);
            this.normalVoterWindow.RegButton.Click += (o, eA) => this.VoterShown();
            this.normalVoterWindow.RegButton.Click += (o, eA) => this.normalVoterWindow.Close();
            this.normalVoterWindow.ShowDialog();
        }

        /// <summary>
        /// When the user presses the lock to go to the unregister window.
        /// </summary>
        public void OpenUnregWindow()
        {
            this.unregWindow = new UnRegVW(model.currentVoter);
            this.unregWindow.UnregisterButton.Click += (o, eA) => this.Unregister(this.unregWindow.AdmPass.Text);
            //unregWindow.UnregisterButton.Click += (o, eA) => unregWindow.Close();
            this.unregWindow.ShowDialog();
        }

        public void OpenLogWindow()
        {
            LogWindow lw = new LogWindow();
            LogModel lm = new LogModel(model.AdminPass, model.SetupInfo.Ip);
            LogController lc = new LogController(lw, lm);
        }

        /// <summary>
        /// Opens a messabe box with a warning. 
        /// </summary>
        /// <param name="msg">The message to be shown.</param>
        public void ShowMessageBox(string msg)
        {
            System.Windows.Forms.MessageBox.Show(msg);
        }

        /// <summary>
        /// Update the setup window with setup information
        /// </summary>
        /// <param name="setupInfo">The setup information that needs to be shown in the setup window.</param>
        private void UpdateSetupWindow(SetupInfo setupInfo)
        {
            this.SetupWindow.TableBox = setupInfo.TableNo.ToString();
            this.SetupWindow.IpTextBox = setupInfo.Ip;
        }

        /// <summary>
        /// Closes the warning window
        /// </summary>
        public void CloseWarningWindow()
        {
            warningWindow.CloseDialog();
            warningWindow = null;
            return;
        }

        /// <summary>
        /// Show a message that the connection to the server has been lost.
        /// </summary>
        private void ConnectionLostMsg()
        {
            this.ShowMessageBox("The connection to the voter box has been lost. Please contact a manager to restore connection.");
        }

        #region Properties.

        public ScannerWindow ScannerWindow
        {
            get { return scannerWindow; }
        }

        public SetupWindow SetupWindow
        {
            get { return setupWindow; }
        }

        public UnRegVW UnregWindow
        {
            get { return this.unregWindow; }
        }

        #endregion

    }

}

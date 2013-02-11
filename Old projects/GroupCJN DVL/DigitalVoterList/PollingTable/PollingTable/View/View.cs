// -----------------------------------------------------------------------
// <copyright file="View.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace DigitalVoterList.PollingTable.View
{

    using DBComm.DBComm.DO;


    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class View
    {
        private Model model;

        private ScannerWindow scannerWindow;

        public View(Model model)
        {
            this.model = model;
            scannerWindow = new ScannerWindow();
            model.CurrentVoterChanged += this.showSpecificVoter;
        }

        public void showSpecificVoter(VoterDO voter)
        {
            ///Ordinary voter window
            //VoterWindow vw1 = new VoterWindow(voter);
            //vw1.Height = 220;
            //vw1.panel1.Controls.Add(new RegAndCancelUC());
            //vw1.Show();

            ///Voter window with error message
            VoterWindow vw = new VoterWindow(voter);
            vw.Height = 280;
            vw.panel1.Controls.Add(new WarningUC());
            vw.Show();

            ///Voter window with unreg functionality
            //VoterWindow vw = new VoterWindow(voter);
            //vw.Height = 340;
            //vw.panel1.Controls.Add(new UnregUC());
            //vw.Show();

        }

        public ScannerWindow ScannerWindow
        {
            get
            {
                return scannerWindow;
            }
        }
    }
}


// -----------------------------------------------------------------------
// <copyright file="Model.cs" company="">
// Author: Claes Martinsen.
// </copyright>
// -----------------------------------------------------------------------

namespace PollingTable.PollingTable.View
{
    using System.Windows.Forms;
    using System.Drawing;

    using DBComm.DBComm.DO;

    /// <summary>
    /// Shows a warning screen when the user has already been registered.
    /// </summary>
    public partial class WarningVW : Form
    {
        public WarningVW(VoterDO voter)
        {
            InitializeComponent();
            
            voterNameLabel.Text = voter.Name;
            voterAddressLabel.Text = voter.Address;
            voterCityLabel.Text = voter.City;

            Size size = new Size(384, 277);
            this.MaximumSize = size;
            this.MinimumSize = size;
        }

        /// <summary>
        /// Closes the form.
        /// </summary>
        public void CloseDialog()
        {
            this.Close();
        }
    }
}

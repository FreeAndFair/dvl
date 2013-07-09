
// -----------------------------------------------------------------------
// <copyright file="Model.cs" company="">
// Author: Claes Martinsen.
// </copyright>
// -----------------------------------------------------------------------

namespace PollingTable.PollingTable.View.Root_elements
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Button user control reused for closing a parent form.
    /// </summary>
    public partial class CancelBtn : UserControl
    {
        public CancelBtn()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The method to be called when the user presses the cancel button.
        /// </summary>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.ParentForm.Close();
            try
            {
                Model.cleanUpDAO();
            }
            catch (Exception)
            {
                MessageBox.Show("Connection lost!");
            }
        }
    }
}

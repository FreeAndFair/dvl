// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StationWindow.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for StationWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="StationWindow.xaml.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace UI {
  using System;
  using System.ComponentModel;
  using System.Linq;
  using System.Net;
  using System.Net.Sockets;
  using System.Windows;
  using System.Windows.Forms;

  using Microsoft.Win32;

  using UI.ManagerWindows;

  /// <summary>
  /// Interaction logic for StationWindow.xaml
  /// </summary>
  public partial class StationWindow {
    #region Fields

    /// <summary>
    /// The _ui.
    /// </summary>
    private readonly UiHandler _ui;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="StationWindow"/> class. 
    /// Constructor
    /// </summary>
    public StationWindow() {
      this.InitializeComponent();
      this._ui = new UiHandler(this);
      this.MainFrame.Navigate(new TypeChoicePage(this.MainFrame, this._ui));
    }

    #endregion

    #region Methods

    /// <summary>
    /// Make sure that the red X in the corner doesn't close the window
    /// </summary>
    /// <param name="e">
    /// the event argument
    /// </param>
    protected override void OnClosing(CancelEventArgs e) {
      if (!CheckMasterPasswordForQuit()) {
        e.Cancel = true;
      }
    }

    /// <summary>
    /// Called when File -&gt; Exit is clicked
    /// </summary>
    /// <param name="sender">
    /// auto generated
    /// </param>
    /// <param name="e">
    /// auto generated
    /// </param>
    private void ExitClick(object sender, RoutedEventArgs e) {
      CheckMasterPasswordForQuit();
    }

    private bool CheckMasterPasswordForQuit() {
      if (_ui._station != null) {
        var d = new CheckMasterPasswordDialog(this._ui, "The master password is required to shut down this station.");
        d.Owner = this;
        d.ShowDialog();

        if (d.IsCancel) {
          return false;
        } else if (d.DialogResult.HasValue &&
                   d.DialogResult == true) {
          Environment.Exit(0);
        } else {
          FlexibleMessageBox.Show(
            "Master password entered incorrctly, please try again.", "Incorrect Master Password", MessageBoxButtons.OK);
          return false;
        }
      } else {
        Environment.Exit(0);
      }
      return true;
    }

    /// <summary>
    /// Called whn File -&gt; Export data is clicked
    /// </summary>
    /// <param name="sender">
    /// auto generated
    /// </param>
    /// <param name="e">
    /// auto generated
    /// </param>
    private void ExportDataClick(object sender, RoutedEventArgs e) {
      var d = new CheckMasterPasswordDialog(this._ui, "The master password is required to export election data.");
      d.Owner = this;
      d.ShowDialog();

      if (d.DialogResult.HasValue &&
          d.DialogResult == true) {
        if (d.IsCancel) return;

        var saveDialog = new Microsoft.Win32.SaveFileDialog { Title = "Generate Reports" };
        saveDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
        saveDialog.ShowDialog();
        if (!saveDialog.FileName.Equals(string.Empty)) this._ui.ExportData(saveDialog.FileName);
      } else
        FlexibleMessageBox.Show(
          "Master password entered incorrectly, please try again.", "Incorrect Master Password", MessageBoxButtons.OK);
    }

    /// <summary>
    /// Called when Help -&gt; User Manual is clicked
    /// </summary>
    /// <param name="sender">
    /// auto generated
    /// </param>
    /// <param name="e">
    /// auto generated
    /// </param>
    private void PollWorkerManualClick(object sender, RoutedEventArgs e) { 
      // System.Diagnostics.Process.Start(@"Manual.pdf"); 
      FlexibleMessageBox.Show("The poll worker manual is not included in this demo,\nbut will automatically open in a new window in the final product.");
    }

    private void SetupManualClick(object sender, RoutedEventArgs e)
    {
      // System.Diagnostics.Process.Start(@"Manual.pdf"); 
      FlexibleMessageBox.Show("The setup manual is not included in this demo,\nbut will automatically open in a new window in the final product.");
    }

    private void VideoClick(object sender, RoutedEventArgs e) {
      System.Diagnostics.Process.Start("fighting_cats4.wmv");
      FlexibleMessageBox.Show("Actual training videos are not available in this demo,\nbut will be listed in and playable from this window in the final product.");
    }

    private void FAQClick(object sender, RoutedEventArgs e) {
      // System.Diagnostics.Process.Start(@"Manual.pdf"); 
      FlexibleMessageBox.Show("The FAQ is not included in this demo,\nbut will automatically open in a new window in the final product.");
    }
    #endregion

    private void ePollbook_Closing(object sender, CancelEventArgs e)
    {

    }
  }
}

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
    protected override void OnClosing(CancelEventArgs e) { e.Cancel = true; }

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
      var d = new CheckMasterPasswordDialog(this._ui);
      d.ShowDialog();

      if (d.DialogResult.HasValue &&
          d.DialogResult == true) {
        if (d.IsCancel) return;

        Environment.Exit(0);
      } else
        MessageBox.Show(
          "Master password entered incorrctly, please try again.", "Incorrect Master Password", MessageBoxButton.OK);
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
      var d = new CheckMasterPasswordDialog(this._ui);
      d.ShowDialog();

      if (d.DialogResult.HasValue &&
          d.DialogResult == true) {
        if (d.IsCancel) return;

        var saveDialog = new SaveFileDialog { Title = "Export Data" };
        saveDialog.Filter = "Data files (*.data)|*.data|All files (*.*)|*.*";
        saveDialog.ShowDialog();
        if (!saveDialog.FileName.Equals(string.Empty)) this._ui.ExportData(saveDialog.FileName);
      } else
        MessageBox.Show(
          "Master password entered incorrectly, please try again.", "Incorrect Master Password", MessageBoxButton.OK);
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
    private void HelpClick(object sender, RoutedEventArgs e) { System.Diagnostics.Process.Start(@"Manual.pdf"); }

    #endregion
  }
}

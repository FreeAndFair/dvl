﻿// --------------------------------------------------------------------------------------------------------------------
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
    /// This window should have an aspect ratio of 1.25.
    /// </summary>
    private const double ASPECT_RATIO = 1.25;

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
      InitializeComponent();
      _ui = new UiHandler(this);
      MainFrame.Navigate(new TypeChoicePage(MainFrame, _ui));
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
    /// Make sure that the window stays at the correct aspect ratio.
    /// </summary>
    /// <param name="sizeInfo">
    /// the information about the size change
    /// </param>
    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
      if (sizeInfo.WidthChanged) {
        this.Width = sizeInfo.NewSize.Height * ASPECT_RATIO;
      } else {
        this.Height = sizeInfo.NewSize.Width * ASPECT_RATIO;
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
        var d = new CheckMasterPasswordDialog(_ui, "The master password is required to shut down this station.");
        d.Owner = this;
        d.ShowDialog();

        if (d.IsCancel) {
          return false;
        } else if (d.DialogResult.HasValue &&
                   d.DialogResult == true) {
          _ui.CloseStation();
          Environment.Exit(0);
        } else {
          FlexibleMessageBox.Show(_ui._stationNativeWindow,
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
      if (_ui._station == null) {
        FlexibleMessageBox.Show(_ui._stationNativeWindow, "There is no election data to report.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        return;
      }
      var d = new CheckMasterPasswordDialog(_ui, "The master password is required to export election data.");
      d.Owner = this;
      d.ShowDialog();

      if (d.DialogResult.HasValue &&
          d.DialogResult == true) {
        if (d.IsCancel) return;

        var saveDialog = new Microsoft.Win32.SaveFileDialog { Title = "Generate Reports" };
        saveDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
        saveDialog.ShowDialog();
        if (!saveDialog.FileName.Equals(string.Empty)) _ui.ExportData(saveDialog.FileName);
        System.Diagnostics.Process.Start(saveDialog.FileName);
      } else
        FlexibleMessageBox.Show(_ui._stationNativeWindow,
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
      FlexibleMessageBox.Show(_ui._stationNativeWindow, "The poll worker manual is not included in this demo,\nbut will automatically open in a new window in the final product.");
    }

    private void SetupManualClick(object sender, RoutedEventArgs e)
    {
      // System.Diagnostics.Process.Start(@"Manual.pdf"); 
      FlexibleMessageBox.Show(_ui._stationNativeWindow, "The setup manual is not included in this demo,\nbut will automatically open in a new window in the final product.");
    }

    private void VideoClick(object sender, RoutedEventArgs e) {
      try {
        System.Diagnostics.Process.Start("Clip.mp4");
      } catch (Exception) { }
      FlexibleMessageBox.Show(_ui._stationNativeWindow, "Actual training videos are not available in this demo,\nbut will be listed in and playable from this window in the final product.");
    }

    private void FAQClick(object sender, RoutedEventArgs e) {
      // System.Diagnostics.Process.Start(@"Manual.pdf"); 
      FlexibleMessageBox.Show(_ui._stationNativeWindow, "The FAQ is not included in this demo,\nbut will automatically open in a new window in the final product.");
    }

    #endregion
  }
}

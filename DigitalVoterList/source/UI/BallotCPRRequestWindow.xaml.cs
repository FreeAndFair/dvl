// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BallotCPRRequestWindow.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for BallotCPRRequestWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="BallotCPRRequestWindow.xaml.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace UI {
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;

  using UI.ManagerWindows;

  /// <summary>
  /// Interaction logic for BallotCPRRequestWindow.xaml
  /// </summary>
  public partial class BallotCPRRequestWindow {
    #region Fields

    /// <summary>
    /// The _ui.
    /// </summary>
    private readonly UiHandler _ui;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="BallotCPRRequestWindow"/> class. 
    /// Constructor
    /// </summary>
    /// <param name="ui">
    /// the UIHandler of the UI
    /// </param>
    public BallotCPRRequestWindow(UiHandler ui) {
      this._ui = ui;
      this._ui.BallotCPRRequestWindow = this;
      this.InitializeComponent();
      this.doneButton.IsEnabled = false;
      this.WaitingLabel.Content = string.Empty;
      this.Focus();
      this.Title = "CPR";
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// Called when the station object replies to a ballot request
    /// </summary>
    /// <param name="succes">
    /// whether a ballot can be handed out or not 
    /// </param>
    public void BallotResponse(bool succes) {
      this._ui.BallotCPRRequestWindow = null;

      if (succes) {
        this.WaitingLabel.Content = string.Empty;
        this.doneButton.IsEnabled = true;
        this.CancelButton.IsEnabled = true;
        MessageBox.Show(
          "Vælgeren " + this.CPRTextbox.Text + " Må gives en stemmeseddel ", 
          "Giv stemmeseddel", 
          MessageBoxButton.OK);
      } else {
        this.WaitingLabel.Content = string.Empty;
        this.doneButton.IsEnabled = true;
        this.CancelButton.IsEnabled = true;
        MessageBox.Show(
          "Vælgeren " + this.CPRTextbox.Text + " Må IKKE gives en stemmeseddel ", 
          "Giv ikke stemmeseddel", 
          MessageBoxButton.OK, 
          MessageBoxImage.Stop);
      }

      this.Close();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Used to check that only number can be typed in the textboxes
    /// </summary>
    /// <param name="text">
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    private static bool IsNumeric(string text) {
      int result;
      return int.TryParse(text, out result);
    }

    /// <summary>
    /// Called when the Text in the CPRTextBox changes
    /// </summary>
    /// <param name="sender">
    /// auto generated
    /// </param>
    /// <param name="e">
    /// auto generated
    /// </param>
    private void CPRTextboxTextChanged(object sender, TextChangedEventArgs e) {
      if (this.CPRTextbox.Text.Length == 10 &&
          this.WaitingLabel.Content.Equals(string.Empty)) this.doneButton.IsEnabled = true;
      else this.doneButton.IsEnabled = false;
    }

    /// <summary>
    /// Called when the cancel button is pressed
    /// </summary>
    /// <param name="sender">
    /// auto generated
    /// </param>
    /// <param name="e">
    /// auto generated
    /// </param>
    private void CancelButtonClick(object sender, RoutedEventArgs e) {
      this._ui.BallotCPRRequestWindow = null;
      this.Close();
    }

    /// <summary>
    /// Called when the Done button is clicked
    /// </summary>
    /// <param name="sender">
    /// auto generated
    /// </param>
    /// <param name="e">
    /// auto generated
    /// </param>
    private void DoneButtonClick(object sender, RoutedEventArgs e) {
      var d = new CheckMasterPasswordDialog(this._ui);
      d.ShowDialog();

      if (d.DialogResult.HasValue &&
          d.DialogResult == true) {
        if (d.IsCancel) return;

        if (!this.CPRTextbox.Text.Equals(string.Empty)) {
          this.WaitingLabel.Content = "Waiting for reply...";
          this.doneButton.IsEnabled = false;
          this.CancelButton.IsEnabled = false;
          this._ui.RequestBallotOnlyCPR(this.CPRTextbox.Text, d.TypedPassword);
        }
      } else
        MessageBox.Show(
          "Det kodeord du indtastede er ikke korret, prøv igen", "Forkert Master Kodeord", MessageBoxButton.OK);
    }

    /// <summary>
    /// When someone pastes something into a textbox this is called
    /// </summary>
    /// <param name="sender">
    /// auto generated
    /// </param>
    /// <param name="e">
    /// auto generated
    /// </param>
    private void PastingHandler(object sender, DataObjectPastingEventArgs e) {
      if (e.DataObject.GetDataPresent(typeof(String))) {
        var text = (String)e.DataObject.GetData(typeof(String));
        if (!IsNumeric(text)) e.CancelCommand();
      } else e.CancelCommand();
    }

    /// <summary>
    /// When someone typed something into a textbox this is called
    /// </summary>
    /// <param name="sender">
    /// auto generated
    /// </param>
    /// <param name="e">
    /// auto generated
    /// </param>
    private void PreviewTextInputHandler(object sender, TextCompositionEventArgs e) { e.Handled = !IsNumeric(e.Text); }

    #endregion
  }
}

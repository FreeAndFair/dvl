// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BallotRequestPage.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for BallotRequestPage.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="BallotRequestPage.xaml.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace UI.StationWindows {
  using System;
  using System.Collections.Generic;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;

  using Aegis_DVL.Database;

  using UI.ManagerWindows;

  /// <summary>
  /// Interaction logic for BallotRequestPage.xaml
  /// </summary>
  public partial class BallotRequestPage {
    #region Fields

    /// <summary>
    /// The blocked.
    /// </summary>
    public bool Blocked;

    /// <summary>
    /// The _parent.
    /// </summary>
    private readonly Frame _parent;

    /// <summary>
    /// The _ui.
    /// </summary>
    private readonly UiHandler _ui;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="BallotRequestPage"/> class. 
    /// Constructor
    /// </summary>
    /// <param name="ui">
    /// the UIHandler for this UI
    /// </param>
    /// <param name="parent">
    /// the frame in which this page is displayed
    /// </param>
    public BallotRequestPage(UiHandler ui, Frame parent) {
      this._ui = ui;
      this._parent = parent;
      this._ui.BallotRequestPage = this;
      this.InitializeComponent();
      this.checkValidityButton.IsEnabled = false;
      this.WaitingLabel.Content = string.Empty;
      this.DriversLicense.Focus();
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// Called when the manager responds to a ballot request
    /// </summary>
    /// <param name="succes">
    /// whether or not the ballot request was a success
    /// </param>
    public void BallotResponse(Voter voter, bool succes) {
      if (voter == null) {
        MessageBox.Show("No voter matches that search criteria.");
      } else {
        this.WaitingLabel.Content = string.Empty;
        string votername;
        string middlename = " ";
        if (voter.MiddleName != null && voter.MiddleName.Trim().Length != 0) {
          middlename = " " + voter.MiddleName + " ";
        }
        votername = voter.FirstName + middlename + voter.LastName;

        if (voter.Suffix != null && voter.Suffix.Trim().Length > 0) {
          votername = votername + ", " + voter.Suffix;
        }

        if (succes) {
          MessageBox.Show(
            votername + " should be given a ballot. ",
            "Give Ballot",
            MessageBoxButton.OK,
            MessageBoxImage.Exclamation);
        } else {
          MessageBox.Show(
            votername + " should NOT be given a ballot. ",
            "Do Not Give Ballot",
            MessageBoxButton.OK,
            MessageBoxImage.Stop);
        }
      }
      this.DriversLicense.Text.Remove(0);
    }

    /// <summary>
    /// Called when this station is promoted
    /// </summary>
    public void BecomeManager() {
      this._ui.BallotRequestPage = null;
      this._parent.Navigate(new ManagerOverviewPage(this._parent, this._ui));
      if (this._ui.EnoughStations()) this._ui.EnoughPeers();
      else this._ui.NotEnoughPeers();
    }

    /// <summary>
    /// Called when the election is ended
    /// </summary>
    public void EndElection() {
      this._ui.BallotRequestPage = null;
      Environment.Exit(0);
    }

    /// <summary>
    /// When the station is told that it has been removed, we navigate to the TypeChoicePage.
    /// </summary>
    public void StationRemoved() {
      this._ui.BallotRequestPage = null;
      this._ui.DisposeStation();
      this._parent.Navigate(new TypeChoicePage(this._parent, this._ui));
    }

    #endregion

    #region Methods

    /// <summary>
    /// Used to check whether a string is purely numbers or not
    /// </summary>
    /// <param name="text">
    /// the string to check
    /// </param>
    /// <returns>
    /// whether a string is purely numbers or not
    /// </returns>
    private static bool IsNumeric(string text) {
      int result;
      return int.TryParse(text, out result);
    }

    /// <summary>
    /// Called when the Done button is pressed
    /// </summary>
    /// <param name="sender">
    /// autogenerated
    /// </param>
    /// <param name="e">
    /// autogenerated
    /// </param>
    private void CheckValidityButtonClick(object sender, RoutedEventArgs e) {
      List<Voter> results = new List<Voter>();
      Int32 stateId;
      bool stateIdParsed = Int32.TryParse(StateId.Text, out stateId);

      if (!DriversLicense.Text.Equals(string.Empty)) {       
        // check driver's license first
        Voter v = _ui._station.Database.GetVoterByDLNumber(DriversLicense.Text);
        if (v != null) {
          results.Add(v);
        }
      } else if (!StateId.Text.Equals(string.Empty) && stateIdParsed) {
        // check state id next
        Voter v = _ui._station.Database.GetVoterByStateId(stateId);
        if (v != null) {
          results.Add(v);
        }
      } else {
        results = _ui._station.Database.GetVotersBySearchStrings
          (LastName.Text, FirstName.Text, MiddleName.Text,
           Address.Text, Municipality.Text, ZipCode.Text);
      }

      Voter choice = null;

      if (results == null || results.Count == 0) {
        MessageBox.Show("No voters match that search criteria.");
      } else if (results.Count == 1) {
        var dialog = new ConfirmSingleVoterDialog(results[0]);
        var result = dialog.ShowDialog();

        if (result.HasValue && result == true) {
          choice = results[0];
        }
      } else {
        MessageBox.Show("Multiple results not handled yet.");
      }

      if (choice != null) {
        WaitingLabel.Content = "Waiting for reply...";
        _ui.RequestBallot(choice.VoterId.ToString());
      }
    }

    /// <summary>
    /// Sees to that only numbers can be pasted in the textbox
    /// </summary>
    /// <param name="sender">
    /// autogenerated
    /// </param>
    /// <param name="e">
    /// autogenerated
    /// </param>
    private void PastingHandler(object sender, DataObjectPastingEventArgs e) {
      if (e.DataObject.GetDataPresent(typeof(String))) {
        var text = (String)e.DataObject.GetData(typeof(String));
        if (!IsNumeric(text)) e.CancelCommand();
      } else e.CancelCommand();
    }

    /// <summary>
    /// Sees to that only numbers can be typed in the textbox
    /// </summary>
    /// <param name="sender">
    /// autogenerated
    /// </param>
    /// <param name="e">
    /// autogenerated
    /// </param>
    private void PreviewTextInputHandler(object sender, TextCompositionEventArgs e) {  }

    /// <summary>
    /// called when the text in the VoterCardNumberTextbox changes
    /// </summary>
    /// <param name="sender">
    /// autogenerated
    /// </param>
    /// <param name="e">
    /// autogenerated
    /// </param>
    private void TextChanged(object sender, TextChangedEventArgs e) {
      if (this.WaitingLabel.Content.Equals(string.Empty) &&
          !this.Blocked) this.checkValidityButton.IsEnabled = true;
      else this.checkValidityButton.IsEnabled = false;
    }

    #endregion
  }
}

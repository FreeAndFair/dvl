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

  using Aegis_DVL.Data_Types;
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
      this.LastName.Focus();
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// Called when the manager responds to a ballot request
    /// </summary>
    /// <param name="succes">
    /// whether or not the ballot request was a success
    /// </param>
    public void BallotResponse(Voter voter, bool success, VoterStatus oldStatus, VoterStatus newStatus) {
      if (!success && newStatus != VoterStatus.NotSeenToday) {
        MessageBox.Show(
          GetFormattedName(voter) + " has already checked in at this location today.",
          "Already Checked In",
          MessageBoxButton.OK,
          MessageBoxImage.Stop);
      }

      switch (newStatus) {
        case VoterStatus.Ineligible:
          MessageBox.Show("According to our records, " + GetFormattedName(voter) +
            " is not eligible to vote until " + voter.EligibleDate.Date.ToString("MM/dd/yyyy"));
          break;
        case VoterStatus.WrongLocation:
          Precinct p = _ui._station.Database.GetPrecinctBySplitId(voter.PrecinctSub);
          if (p.Address.Trim().Length > 0) {
            var wrd = new WrongLocationDialog(voter, p);
            var result = wrd.ShowDialog();
            if (result.HasValue && result == true) {
              string here = _ui._station.PollingPlace.Address + ", " + _ui._station.PollingPlace.CityStateZIP;
              string there = p.Address + ", " + p.CityStateZIP;
              MessageBox.Show("Directions from " + here + " to " + there + " are not available in this demo, " +
                "but will be available in the final product.");
            }
          } else {
            MessageBox.Show("The precinct where " + GetFormattedName(voter) + " is registered is" +
              " not participating in this election.");
          }
          break;
        case VoterStatus.VotedByMail:
          MessageBox.Show("According to our records, " + GetFormattedName(voter) +
            " has already submitted a vote by mail and should not receive a ballot.");
          break;
        case VoterStatus.SuspendedVoter:
          MessageBox.Show("According to our records, " + GetFormattedName(voter) +
            " is a suspense voter and requires special processing before receiving a ballot.");
          break;
        case VoterStatus.OutOfCounty:
          MessageBox.Show("According to our records, " + GetFormattedName(voter) +
            " is an out-of-county voter.");
          break;
        case VoterStatus.EarlyVotedInPerson:
          MessageBox.Show("According to our records, " + GetFormattedName(voter) +
            " has already voted at an early voting location.");
          break;
        case VoterStatus.AbsenteeVotedInPerson:
          MessageBox.Show("According to our records, " + GetFormattedName(voter) +
            " registered as an absentee voter but did not receive their ballot.");
          break;
        case VoterStatus.MailBallotNotReturned:
          MessageBox.Show("According to our records, " + GetFormattedName(voter) +
            " was sent an absentee ballot and has not yet returned it. Please obtain " +
            " the appropriate affidavit before providing a ballot.");
          break;
        default:
          break;
      }

      if (success) {
        switch ((VoterStatus)voter.PollbookStatus) {
          case VoterStatus.SuspendedVoter:
          case VoterStatus.OutOfCounty:
          case VoterStatus.MailBallotNotReturned:
          case VoterStatus.ActiveVoter:
          case VoterStatus.Provisional:
            var bd = new GiveBallotDialog(voter);
            bd.ShowDialog();
            break;

          default:
            break;
        }
      }

      this.EnableFields(true);
      this.ClearFields();
      this.WaitingLabel.Content = string.Empty;
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
      MessageBox.Show("The election has ended. This station is shutting down.");
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

    public void RecoverFromManagerChange() {
      if (!FirstName.IsEnabled) {
        // we were in the middle of a query to which an answer is not going to come
        EnableFields(true);
        WaitingLabel.Content = "Could not reach manager, please try again.";
        UpdateButtonState();
      }
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
        MessageBox.Show("No voters match your search. Please try again or have the voter register for a provisional ballot.");
      } else if (results.Count == 1) {
        var dialog = new ConfirmSingleVoterDialog(results[0]);
        var result = dialog.ShowDialog();

        if (result.HasValue && result == true) {
          choice = results[0];
        }
      } else {
        var dialog = new ConfirmMultiVoterDialog(results);
        var result = dialog.ShowDialog();

        if (result.HasValue && result == true) {
          choice = dialog.SelectedVoter;
        }
      }

      if (choice != null) {
        WaitingLabel.Content = "Waiting for response from manager...";
        EnableFields(false);
        VoterStatus vs = GetNewVoterStatus(choice);
        if (vs == (VoterStatus) choice.PollbookStatus) {
          BallotResponse(choice, false, vs, vs);
        } else {
          _ui.RequestStatusChange(choice, vs);
        }
      }
    }

    private void EnableFields(bool enabled) {
      LastName.IsEnabled = enabled;
      FirstName.IsEnabled = enabled;
      MiddleName.IsEnabled = enabled;
      Address.IsEnabled = enabled;
      Municipality.IsEnabled = enabled;
      ZipCode.IsEnabled = enabled;
      DriversLicense.IsEnabled = enabled;
      StateId.IsEnabled = enabled;
      checkValidityButton.IsEnabled = enabled;
    }

    private void ClearFields() {
      LastName.Text = "";
      FirstName.Text = "";
      MiddleName.Text = "";
      Address.Text = "";
      Municipality.Text = "";
      ZipCode.Text = "";
      DriversLicense.Text = "";
      StateId.Text = "";
      LastName.Focus();
    }

    private VoterStatus GetNewVoterStatus(Voter voter) {
      // let's figure out what the voter's status should be
      VoterStatus result = VoterStatus.Unavailable;
      if (voter.EligibleDate.Date > DateTime.Today.Date) {
        // voter is ineligible
        result = VoterStatus.Ineligible;
      } else if (!_ui._station.PollingPlace.PrecinctIds.Contains(voter.PrecinctSub)) {
        // voter doesn't belong here
        result = VoterStatus.WrongLocation;
      } else if (voter.Status.ToUpper().Equals("A")) {
        // active voter
        if (voter.Voted) {
          result = VoterStatus.EarlyVotedInPerson;
        } else if (voter.Absentee && voter.ReturnStatus.ToUpper().Equals("PB")) {
          result = VoterStatus.VotedByMail;
        } else if (voter.Absentee && voter.ReturnStatus.Trim().Length == 0) {
          result = VoterStatus.MailBallotNotReturned;
        } else if (voter.Absentee) {
          result = VoterStatus.AbsenteeVotedInPerson;
        } else if (voter.PrecinctSub.StartsWith("0")) {
          result = VoterStatus.OutOfCounty;
        } else {
          result = VoterStatus.ActiveVoter;
        }
      } else {
        // suspended voter
        result = VoterStatus.SuspendedVoter;
      }

      return result;
    }

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
      UpdateButtonState();
    }

    private void UpdateButtonState() {
      if (this.WaitingLabel.Content.Equals(string.Empty) &&
          !this.Blocked) this.checkValidityButton.IsEnabled = true;
      else this.checkValidityButton.IsEnabled = false;
    }

    private string GetFormattedName(Voter voter) {
      string votername;
      string middlename = " ";
      if (voter.MiddleName != null && voter.MiddleName.Trim().Length != 0) {
        middlename = " " + voter.MiddleName + " ";
      }
      votername = voter.FirstName + middlename + voter.LastName;

      if (voter.Suffix != null && voter.Suffix.Trim().Length > 0) {
        votername = votername + ", " + voter.Suffix;
      }
      return votername;
    }


    #endregion
  }
}

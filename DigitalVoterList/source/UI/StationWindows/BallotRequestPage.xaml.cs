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
  using System.Windows.Forms;
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

    private bool Waiting;

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
      _ui = ui;
      _parent = parent;
      _ui.BallotRequestPage = this;
      InitializeComponent();
      checkValidityButton.IsEnabled = false;
      WaitingLabel.Content = string.Empty;
      StateId.Focus();
      Blocked = false;
      Waiting = false;
      IPLabel.Content = IPLabel.Content.ToString().Replace("255.255.255.255", _ui.IdentifyingString());
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
      if (!success && newStatus != VoterStatus.NotSeenToday && newStatus != VoterStatus.WrongLocation) {
        string message;
        if (newStatus == VoterStatus.ActiveVoter || newStatus == VoterStatus.SuspendedVoter ||
            newStatus == VoterStatus.OutOfCounty || newStatus == VoterStatus.MailBallotNotReturned) {
          message = GetFormattedName(voter) + "\nhas already checked in and received a ballot\n(style " +
            voter.BallotStyle + ") at this location today.";
        } else {
          message = GetFormattedName(voter) + "\nhas already checked in and did not receive a ballot\n" +
            "at this location today.";
        }
        FlexibleMessageBox.Show(
          message,
          "Already Checked In",
          MessageBoxButtons.OK,
          MessageBoxIcon.Stop);
      }

      switch (newStatus) {
        case VoterStatus.Ineligible:
          FlexibleMessageBox.Show("According to our records,\n" + GetFormattedName(voter) +
            "\nis not eligible to vote until " + voter.EligibleDate.Date.ToString("MM/dd/yyyy"));
          break;
        case VoterStatus.WrongLocation:
          Precinct p = _ui._station.Database.GetPrecinctBySplitId(voter.PrecinctSub);
          if (p.Address.Trim().Length > 0) {
            WrongLocationDialog wrd = null;
            Boolean result = false;
            _ui._stationWindow.Dispatcher.Invoke(
              System.Windows.Threading.DispatcherPriority.Normal,
              new Action(
                delegate {
                  wrd = new WrongLocationDialog(voter, p);
                  wrd.Owner = _ui._stationWindow;
                  result = (Boolean)wrd.ShowDialog();
                }));
            if (result == true) {
              string here = _ui._station.PollingPlace.Address + ", " + _ui._station.PollingPlace.CityStateZIP;
              string there = p.Address + ", " + p.CityStateZIP;
              string url = "https://maps.google.com/maps/dir/" + here.Replace(" ", "+") + "/" + there.Replace(" ", "+");
              System.Diagnostics.Process.Start(url);
            }
          } else {
            FlexibleMessageBox.Show("The precinct where\n" + GetFormattedName(voter) + "\nis registered is" +
              " not participating\nin this election.");
          }
          break;
        case VoterStatus.VotedByMail:
          FlexibleMessageBox.Show("According to our records,\n" + GetFormattedName(voter) +
            "\nhas already submitted a vote by mail\nand should not receive a ballot.");
          break;
        case VoterStatus.EarlyVotedInPerson:
          FlexibleMessageBox.Show("According to our records,\n" + GetFormattedName(voter) +
            "\nhas already voted at an early voting location.");
          break;
        case VoterStatus.AbsenteeVotedInPerson:
          FlexibleMessageBox.Show("According to our records,\n" + GetFormattedName(voter) +
            "\nregistered as an absentee voter but did not receive their ballot.");
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
            _ui._stationWindow.Dispatcher.Invoke(
              System.Windows.Threading.DispatcherPriority.Normal,
              new Action(
                delegate {
                  var bd = new GiveBallotDialog(voter);
                  bd.Owner = _ui._stationWindow;
                  bd.ShowDialog();
                }));
            break;

          default:
            break;
        }
      }

      EnableFields(true);
      ClearFields();
      WaitingLabel.Content = string.Empty;
      Waiting = false;
    }

    /// <summary>
    /// Called when this station is promoted
    /// </summary>
    public void BecomeManager() {
      _ui.BallotRequestPage = null;
      _parent.Navigate(new ManagerOverviewPage(_parent, _ui));
      if (_ui.EnoughStations()) _ui.EnoughPeers();
      else _ui.NotEnoughPeers();
    }

    /// <summary>
    /// Called when the election is ended
    /// </summary>
    public void EndElection() {
      _ui.BallotRequestPage = null;
      FlexibleMessageBox.Show("The election has ended. This station is shutting down.");
      Environment.Exit(0);
    }

    /// <summary>
    /// When the station is told that it has been removed, we navigate to the TypeChoicePage.
    /// </summary>
    public void StationRemoved() {
      _ui.BallotRequestPage = null;
      _ui.DisposeStation();
      _parent.Navigate(new TypeChoicePage(_parent, _ui));
    }

    public void RecoverFromManagerChange() {
      if (Waiting) {
        // we were in the middle of a query to which an answer is not going to come
        EnableFields(true);
        WaitingLabel.Content = "Manager changed to " + _ui._station.Manager.Address + ", please try again.";
        Waiting = false;
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
      Int64 stateId = -1;
      bool stateIdParsed = Int64.TryParse(StateId.Text.Trim().TrimEnd(System.Environment.NewLine.ToCharArray()), out stateId);

      if (!StateId.Text.Equals(string.Empty)) {
        // check state id first
        Voter v = _ui._station.Database.GetVoterByStateId(stateId);
        if (v != null) {
          results.Add(v);
        }
      }
      else if (!DriversLicense.Text.Equals(string.Empty)) {       
        // check driver's license first
        Voter v = _ui._station.Database.GetVoterByDLNumber(DriversLicense.Text);
        if (v != null) {
          results.Add(v);
        }
      } else {
        results = _ui._station.Database.GetVotersBySearchStrings
          (LastName.Text, FirstName.Text, MiddleName.Text,
           Address.Text, Municipality.Text, ZipCode.Text);
      }

      Voter choice = null;
      VoterStatus vs = VoterStatus.Unavailable;

      if (results == null || results.Count == 0) {
        FlexibleMessageBox.Show("No voters match your search. Please try again\nor have the voter register for a provisional ballot.");
      } else if (results.Count == 1) {
        vs = GetNewVoterStatus(results[0]);
        Window dialog;
        Boolean result = false;
        _ui._stationWindow.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal,
          new Action(
            delegate {
              if (((int)vs != results[0].PollbookStatus) && ((vs == VoterStatus.SuspendedVoter) ||
                                                            (vs == VoterStatus.MailBallotNotReturned) ||
                                                            (vs == VoterStatus.OutOfCounty))) {
                dialog = new ConfirmSingleVoterWithConditionsDialog(results[0], vs);
              } else {
                dialog = new ConfirmSingleVoterDialog(results[0]);
              }
              dialog.Owner = _ui._stationWindow;
              result = (Boolean)dialog.ShowDialog();
            }));
        if (result) {
          choice = results[0];
        }
      } else {
        ConfirmMultiVoterDialog dialog = null;
        Boolean result = false;

        _ui._stationWindow.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal,
          new Action(
            delegate {
              dialog = new ConfirmMultiVoterDialog(results);
              dialog.Owner = _ui._stationWindow;
              result = (Boolean)dialog.ShowDialog();
            }));

        if (result) {
          choice = dialog.SelectedVoter;
          vs = GetNewVoterStatus(choice);
          Window dialog2;
          Boolean result2 = false;

          _ui._stationWindow.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal,
            new Action(
              delegate {
                if (((int)vs != choice.PollbookStatus) && ((vs == VoterStatus.SuspendedVoter) ||
                                                                      (vs == VoterStatus.MailBallotNotReturned) ||
                                                                      (vs == VoterStatus.OutOfCounty))) {
                  dialog2 = new ConfirmSingleVoterWithConditionsDialog(choice, vs);
                } else {
                  dialog2 = new ConfirmSingleVoterDialog(choice);
                }
                dialog2.Owner = _ui._stationWindow;
                result2 = (Boolean)dialog2.ShowDialog();
              }));
          if (!result2) {
            choice = null;
            vs = VoterStatus.Unavailable;
          }
        }
      }

      if (choice != null) {
        WaitingLabel.Content = "Waiting for response from manager...";
        EnableFields(false);
        Waiting = true;
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
      StateId.Focus();
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
      if (!Blocked && !Waiting) checkValidityButton.IsEnabled = true;
      else checkValidityButton.IsEnabled = false;
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

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {

    }
  }
}

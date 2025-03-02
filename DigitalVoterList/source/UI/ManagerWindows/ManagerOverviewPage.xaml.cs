﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagerOverviewPage.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for ManagerOverviewPage.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="ManagerOverviewPage.xaml.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace UI.ManagerWindows {
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net;
  using System.Threading;
  using System.Windows;
  using System.Windows.Forms;
  using System.Windows.Controls;
  using System.Windows.Input;

  using Aegis_DVL.Database;
  using Aegis_DVL.Data_Types;

  using UI.Data;
  using UI.StationWindows;

  /// <summary>
  /// Interaction logic for ManagerOverviewPage.xaml
  /// </summary>
  public partial class ManagerOverviewPage {
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

    /// <summary>
    /// The _active update thread.
    /// </summary>
    private Thread _activeUpdateThread;

    private HashSet<int> _ballotStatuses = new HashSet<int>();

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ManagerOverviewPage"/> class. 
    /// Constructor
    /// </summary>
    /// <param name="parent">
    /// the frame which tihs page is displayed in
    /// </param>
    /// <param name="ui">
    /// the UIHandler for this UI
    /// </param>
    public ManagerOverviewPage(Frame parent, UiHandler ui) {
      InitializeComponent();
      _parent = parent;
      _ui = ui;
      _ui.ManagerOverviewPage = this;

      LoadingBar.Visibility = System.Windows.Visibility.Hidden;
      LoadingBar.Value = 100;
      RemoveButton.IsEnabled = false;
      AddButton.IsEnabled = false;
      EndElectionButton.IsEnabled = false;
      IPLabel.Content = IPLabel.Content.ToString().Replace("255.255.255.255", ui._station.Communicator.GetIdentifyingString());
      ManagerstationGrid.ItemsSource = _ui._station.PeerStatuses.Values;

      // Change the width of the window
      var wnd = Window.GetWindow(_parent);
      if (wnd != null) wnd.Width = 1000;
      PopulateList();
      RefreshStatistics();
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// After this manager tries to connect to a station, the station replies.
    /// This method is called when the reply is received.
    /// </summary>
    /// <param name="ip">
    /// the IP adress of the replying station
    /// </param>
    /// <returns>
    /// the password the user has typed in the AcceptStationDialog
    /// </returns>
    public string IncomingReply(IPEndPoint ip) {
      AcceptStationDialog acd = null; 
      Boolean result = false; 

      _ui._stationWindow.Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal,
        new Action (
          delegate {
            acd = new AcceptStationDialog(ip, _ui);
            acd.Owner = _ui._stationWindow;
            result = (Boolean) acd.ShowDialog();
          }));

      if (result == true) return acd.TypedPassword;

      return string.Empty;
    }

    /// <summary>
    /// Marks a machine as connected in the list
    /// </summary>
    /// <param name="ip">
    /// the ip address of the machine
    /// </param>
    public void RefreshGrid() {
      ManagerstationGrid.Items.Refresh();
      UpdateControls();
    }

    /// <summary>
    /// Populates the list with the appropiate machines
    /// </summary>
    public void PopulateList() {
      if (_activeUpdateThread != null) return;
      EndElectionButton.IsEnabled = false;
      RefreshButton.IsEnabled = false;
      Thread oThread = new Thread(PopulateListThread);
      _activeUpdateThread = oThread;
      oThread.Start();
    }

    /// <summary>
    /// the thread to populate the list
    /// </summary>
    /// <param name="mvp">
    /// this manager overview page
    /// </param>
    public void PopulateListThread() {
      Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, 
        new Action(delegate { UpdateLabel.Content = "Scanning..."; }));
      Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, 
        new Action(delegate { LoadingBar.Visibility = System.Windows.Visibility.Visible; }));

      _ui.DiscoverPeers();
      Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal,
        new Action(delegate { ManagerstationGrid.Items.Refresh(); }));
      Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate { UpdateLabel.Content = string.Empty; }));
      Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, 
        new Action(delegate { LoadingBar.Visibility = System.Windows.Visibility.Hidden; }));
      Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, 
        new Action(delegate { RefreshButton.IsEnabled = true; }));
      Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, 
        new Action(delegate { EndElectionButton.IsEnabled = true; }));
      _activeUpdateThread = null;
    }

    /// <summary>
    /// Sets the password label
    /// </summary>
    /// <param name="content">
    /// the new content of the label
    /// </param>
    public void SetPasswordLabel(string content) { PasswordLabel.Text = content; }

    /// <summary>
    /// Unmark a connected station in the list
    /// </summary>
    /// <param name="ip">the IP address of the station to unmark</param>
    public void UnmarkSelectedStation() {
      StationStatus ss = (StationStatus)ManagerstationGrid.SelectedItem;
      if (ss != null) {
        ss.ConnectionState = "Not Connected";
      }
      ManagerstationGrid.Items.Refresh();
    }

    public void ConvertToStation() {
      Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal,
        new Action(delegate {
        if (_activeUpdateThread != null) _activeUpdateThread.Abort();
        _ui.ManagerOverviewPage = null;
        _parent.Navigate(new BallotRequestPage(_ui, _parent));
      }));
    }

    public void RefreshStatistics() {
      IEnumerable<Voter> voters = _ui._station.Database.AllVoters;

      Known.Content = "Known Voters: " + voters.Count();
      Eligible.Content = "Registered to Vote Here: " + voters.Where(voter => _ui._station.PollingPlace.PrecinctIds.Contains(voter.PrecinctSub)).Count();
      CheckedIn.Content = "Checked In: " + voters.Where(voter => voter.PollbookStatus != (int)VoterStatus.NotSeenToday).Count();
      Active.Content = "Active: " + voters.Where(voter => voter.PollbookStatus == (int)VoterStatus.ActiveVoter).Count();
      Suspended.Content = "Suspended: " + voters.Where(voter => voter.PollbookStatus == (int)VoterStatus.SuspendedVoter).Count();
      OutOfCounty.Content = "Out of County: " + voters.Where(voter => voter.PollbookStatus == (int)VoterStatus.OutOfCounty).Count();
      WrongLocation.Content = "Wrong Location: " + voters.Where(voter => voter.PollbookStatus == (int)VoterStatus.WrongLocation).Count();
      EarlyVoted.Content = "Already Voted Early: " + voters.Where(voter => voter.PollbookStatus == (int)VoterStatus.EarlyVotedInPerson).Count();
      AbsenteeVoted.Content = "Already Voted Absentee: " + voters.Where(voter => voter.PollbookStatus == (int)VoterStatus.VotedByMail).Count();
      MailBallot.Content = "Mail Ballot Not Returned: " + voters.Where(voter => voter.PollbookStatus == (int)VoterStatus.MailBallotNotReturned).Count();
      Ineligible.Content = "Otherwise Ineligible: " + voters.Where(voter => voter.PollbookStatus == (int)VoterStatus.Ineligible).Count();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Checks whether a string is purely numeric or not
    /// </summary>
    /// <param name="text">
    /// the string to check
    /// </param>
    /// <returns>
    /// whether a string is purely numeric or not
    /// </returns>
    private static bool IsNumeric(string text) {
      int result;
      return int.TryParse(text, out result);
    }

    /// <summary>
    /// Called when the add button is clicked
    /// </summary>
    /// <param name="sender">
    /// autogenerated
    /// </param>
    /// <param name="e">
    /// autogenerated
    /// </param>
    private void AddButtonClick(object sender, RoutedEventArgs e) {
      if (ManagerstationGrid.SelectedCells.Count != 0) {
        _ui.ExchangeKeys(((StationStatus)ManagerstationGrid.SelectedItem).Address);
      }
      UpdateControls();
    }

    /// <summary>
    /// Called when the end election button is clicked
    /// </summary>
    /// <param name="sender">
    /// autogenerated
    /// </param>
    /// <param name="e">
    /// autogenerated
    /// </param>
    private void EndElectionButtonClick(object sender, RoutedEventArgs e) {
      Boolean result = false;
      Boolean cancel = false;
      _ui._stationWindow.Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal,
        new Action(
          delegate {
            var d = new CheckMasterPasswordDialog(_ui, "The master password is required to end the election.");
            d.Owner = _ui._stationWindow;
            result = (Boolean)d.ShowDialog();
            cancel = d.IsCancel;
          }));

      if (cancel) return;

      if (result) {
        if (_activeUpdateThread != null) _activeUpdateThread.Abort();

        _ui.AnnounceEndElection();
        _ui.ManagerOverviewPage = null;
        _parent.Navigate(new EndedElectionPage(_parent, _ui));
      } else {
        FlexibleMessageBox.Show(_ui._stationNativeWindow,
          "You have entered an incorrect master password, please try again.", "Incorrect Master Password", MessageBoxButtons.OK);
      }
    }

    /// <summary>
    /// Called when the make manager button is clicked
    /// </summary>
    /// <param name="sender">
    /// autogenerated
    /// </param>
    /// <param name="e">
    /// autogenerated
    /// </param>
    private void MakeManagerButtonClick(object sender, RoutedEventArgs e) {
      Boolean result = false;
      Boolean cancel = false;
      _ui._stationWindow.Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal,
        new Action(
          delegate {
            var d = new CheckMasterPasswordDialog(_ui, "The master password is required to promote a check-in station to a manager.");
            d.Owner = _ui._stationWindow;
            result = (Boolean)d.ShowDialog();
            cancel = d.IsCancel;
          }));

      if (cancel) return;

      if (result) {
        if (ManagerstationGrid.SelectedItem != null &&
            _ui.MakeManager(((StationStatus)ManagerstationGrid.SelectedItem).Address)) {
        } else {
          FlexibleMessageBox.Show(_ui._stationNativeWindow, "Could not connect to the specified station", "No Connection", MessageBoxButtons.OK);
        }
      } else {
        FlexibleMessageBox.Show(_ui._stationNativeWindow,
          "You have entered an incorrect master password, please try again.", 
          "Incorrect Master Password", 
          MessageBoxButtons.OK, 
          MessageBoxIcon.Stop);
      }
    }

    /// <summary>
    /// The managerstation grid selection changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ManagerstationGridSelectionChanged(object sender, SelectionChangedEventArgs e) {
      UpdateControls();
    }

    public void UpdateControls() {
      AddButton.IsEnabled = false;
      RemoveButton.IsEnabled = false;
      MakeManagerButton.IsEnabled = false;

      if (ManagerstationGrid.SelectedItem != null) {
        StationStatus ss = (StationStatus)ManagerstationGrid.SelectedItem;
        if (ss.Connected()) {
          AddButton.IsEnabled = false;
          RemoveButton.IsEnabled = true;
          MakeManagerButton.IsEnabled = true;
        } else if (ss.Synchronizing()) {
          AddButton.IsEnabled = false;
          RemoveButton.IsEnabled = false;
          MakeManagerButton.IsEnabled = false;
        } else {
          AddButton.IsEnabled = true;
          RemoveButton.IsEnabled = false;
          MakeManagerButton.IsEnabled = false;
        }
      }
    }

    /// <summary>
    /// Sees to that the textboxes only accepts numbers when something is pasted
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
    /// Sees to that the textboxes only accepts numbers
    /// </summary>
    /// <param name="sender">
    /// autogenerated
    /// </param>
    /// <param name="e">
    /// autogenerated
    /// </param>
    private void PreviewTextInputHandler(object sender, TextCompositionEventArgs e) { e.Handled = !IsNumeric(e.Text); }

    /// <summary>
    /// Called when the refresh button is clicked
    /// </summary>
    /// <param name="sender">
    /// autogenerated
    /// </param>
    /// <param name="e">
    /// autogenerated
    /// </param>
    private void RefreshButtonClick(object sender, RoutedEventArgs e) { PopulateList(); }

    /// <summary>
    /// Called when the remove button is clicked
    /// </summary>
    /// <param name="sender">
    /// autogenerated
    /// </param>
    /// <param name="e">
    /// autogenerated
    /// </param>
    private void RemoveButtonClick(object sender, RoutedEventArgs e) {
      if (((StationStatus)ManagerstationGrid.SelectedItem).Connected()) {
        _ui.RemoveStation(((StationStatus)ManagerstationGrid.SelectedItem).Address);
        UnmarkSelectedStation();
        Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal,
          new Action(
            delegate { 
              ManagerstationGrid.Items.Refresh(); 
            }));
      }
    }

    #endregion
  }
}

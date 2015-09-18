// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OverviewPage.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for OverviewPage.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="OverviewPage.xaml.cs" company="DemTech">
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

  using Aegis_DVL.Data_Types;
  using UI.Data;

  /// <summary>
  /// Interaction logic for OverviewPage.xaml
  /// </summary>
  public partial class OverviewPage {
    #region Fields

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

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="OverviewPage"/> class. 
    /// Constructor
    /// </summary>
    /// <param name="parent">
    /// the frame this page is shown in
    /// </param>
    /// <param name="ui">
    /// the UIHandler for this UI
    /// </param>
    public OverviewPage(Frame parent, UiHandler ui) {
      InitializeComponent();
      _parent = parent;
      _ui = ui;
      _ui.OverviewPage = this;
      LoadingBar.Visibility = System.Windows.Visibility.Hidden;
      LoadingBar.Value = 100;
      RemoveButton.IsEnabled = false;
      AddButton.IsEnabled = false;
      StartEndElectionButton.IsEnabled = false;
      IPLabel.Content = IPLabel.Content.ToString().Replace("255.255.255.255", ui._station.Communicator.GetIdentifyingString());
      stationGrid.ItemsSource = _ui._station.PeerStatuses.Values;
      PopulateList();
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
        new Action(
          delegate {
            acd = new AcceptStationDialog(ip, _ui);
            acd.Owner = _ui._stationWindow;
            result = (Boolean)acd.ShowDialog();
          }));

      if (result == true) return acd.TypedPassword;

      return string.Empty;
    }

    /// <summary>
    /// Marks a station as connected in the list
    /// </summary>
    /// <param name="ip">
    /// the IP address of the station to mark
    /// </param>
    public void RefreshGrid() {
      stationGrid.Items.Refresh();
    }

    /// <summary>
    /// Populates this lists with the appropiate stations
    /// </summary>
    public void PopulateList() {
      StartEndElectionButton.IsEnabled = false;

      if (_activeUpdateThread != null) _activeUpdateThread.Abort();

      RefreshButton.IsEnabled = false;
      Thread oThread = new Thread(() => PopulateListThread(this));
      _activeUpdateThread = oThread;
      oThread.Start();
    }

    /// <summary>
    /// The thread that updates the list
    /// </summary>
    /// <param name="ovp">
    /// this overview page
    /// </param>
    public void PopulateListThread(OverviewPage ovp) {
      StationStatus selected = null;

      ovp.Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal,
        new Action(delegate { selected = (StationStatus)stationGrid.SelectedItem; }));

      ovp.Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, 
        new Action(delegate { UpdateLabel.Content = "Scanning..."; }));
      ovp.Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, 
        new Action(delegate { LoadingBar.Visibility = System.Windows.Visibility.Visible; }));
      _ui.DiscoverPeers();

      ovp.Dispatcher.Invoke(
         System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate { stationGrid.Items.Refresh(); }));
        
      ovp.Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate { UpdateLabel.Content = string.Empty; }));
      ovp.Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, 
        new Action(delegate { LoadingBar.Visibility = System.Windows.Visibility.Hidden; }));
      ovp.Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal,
        new Action(delegate { RefreshButton.IsEnabled = true; UpdateControls(); }));
    }

    /// <summary>
    /// Sets the password label
    /// </summary>
    /// <param name="content">
    /// the string to set the label to
    /// </param>
    public void SetPasswordLabel(string content) { PasswordLabel.Content = content; }

    #endregion

    #region Methods

    /// <summary>
    /// Called when the Add button is clicked
    /// </summary>
    /// <param name="sender">
    /// autogenerated
    /// </param>
    /// <param name="e">
    /// autogenerated
    /// </param>
    private void AddButtonClick(object sender, RoutedEventArgs e) {
      if (stationGrid.SelectedCells.Count != 0) {
        IPEndPoint ipep = ((StationStatus)stationGrid.SelectedItem).Address;
        if (!_ui.GetPeerlist().Contains(ipep)) {
          _ui.ExchangeKeys(ipep);
        }
      }
      UpdateControls();
    }

    /// <summary>
    /// Called when the back button is clicked
    /// </summary>
    /// <param name="sender">
    /// autogenerated
    /// </param>
    /// <param name="e">
    /// autogenerated
    /// </param>
    private void BackButtonClick(object sender, RoutedEventArgs e) {
      if (_activeUpdateThread != null) _activeUpdateThread.Abort();

      _parent.Navigate(new DataLoadPage(_parent, _ui));
      _ui.DisposeStation();
      _ui.OverviewPage = null;
    }

    /// <summary>
    /// Called when the refresh button is clicked
    /// </summary>
    /// <param name="sender">
    /// autogenerated
    /// </param>
    /// <param name="e">
    /// autogenerated
    /// </param>
    private void RefreshButtonClick(object sender, RoutedEventArgs e) { PopulateList(); UpdateControls(); }

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
      if (stationGrid.SelectedItem != null) {
        if (((StationStatus)stationGrid.SelectedItem).Connected()) {
          _ui.RemoveStation(((StationStatus)stationGrid.SelectedItem).Address);
          PopulateList();
        }
      }
      UpdateControls();
    }

    /// <summary>
    /// Called when the start election button is clicked 
    /// </summary>
    /// <param name="sender">
    /// autogenerated
    /// </param>
    /// <param name="e">
    /// autogenerated
    /// </param>
    private void StartEndElectionButtonClick(object sender, RoutedEventArgs e) {
      if (!_ui.EnoughStations()) {
        FlexibleMessageBox.Show(_ui._stationNativeWindow,
          "You are not connected to enough stations to start the election.", 
          "Not Enough Stations", 
          MessageBoxButtons.OK, 
          MessageBoxIcon.Information);
        return;
      }

      Boolean result = false;
      Boolean cancel = false;
      _ui._stationWindow.Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal,
        new Action(
          delegate {
            var d = new CheckMasterPasswordDialog(_ui, "The master password is required to start the election.");
            d.Owner = _ui._stationWindow;
            result = (Boolean)d.ShowDialog();
            cancel = d.IsCancel;
          }));

      if (result) {
        if (cancel) return;

        if (_activeUpdateThread != null) _activeUpdateThread.Abort();

        _ui.OverviewPage = null;
        _ui.ManagerAnnounceStartElection();
        _parent.Navigate(new ManagerOverviewPage(_parent, _ui));
      } else
        FlexibleMessageBox.Show(_ui._stationNativeWindow,
          "Incorrect master password, please try again.", "Incorrect Master Password", MessageBoxButtons.OK);
    }

    /// <summary>
    /// The station grid selection changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void StationGridSelectionChanged(object sender, SelectionChangedEventArgs e) {
      UpdateControls();
    }

    public void UpdateControls() {
      if ((stationGrid.SelectedItem != null) &&
          ((StationStatus)stationGrid.SelectedItem).Connected()) {
        AddButton.IsEnabled = false;
        RemoveButton.IsEnabled = true;
      } else {
        AddButton.IsEnabled = true;
        RemoveButton.IsEnabled = false;
      }

      StartEndElectionButton.IsEnabled = false;

      foreach (StationStatus s in stationGrid.ItemsSource) {
        if (s.Connected()) {
          StartEndElectionButton.IsEnabled = true;
        }
      }
    }

    #endregion
  }
}

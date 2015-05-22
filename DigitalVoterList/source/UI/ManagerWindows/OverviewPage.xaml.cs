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
  using System.Windows.Controls;

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
      this.InitializeComponent();
      this._parent = parent;
      this._ui = ui;
      this._ui.OverviewPage = this;
      this.LoadingBar.Visibility = Visibility.Hidden;
      this.LoadingBar.Value = 100;
      this.RemoveButton.IsEnabled = false;
      this.AddButton.IsEnabled = false;
      this.StartEndElectionButton.IsEnabled = false;
      this.PopulateList();
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
      var acd = new AcceptStationDialog(ip, this._ui);
      var result = acd.ShowDialog();

      if (result.HasValue &&
          result == true) return acd.TypedPassword;

      return string.Empty;
    }

    /// <summary>
    /// Marks a station as connected in the list
    /// </summary>
    /// <param name="ip">
    /// the IP address of the station to mark
    /// </param>
    public void MarkAsConnected(IPEndPoint ip) {
      foreach (StationStatus s in this.stationGrid.Items) {
        if (s.IpAddress == ip.Address.ToString()) {
          s.ConnectionState = "Connected";
        }
      }
      this.stationGrid.Items.Refresh();
    }

    /// <summary>
    /// Populates this lists with the appropiate stations
    /// </summary>
    public void PopulateList() {
      this.StartEndElectionButton.IsEnabled = false;

      if (this._activeUpdateThread != null) this._activeUpdateThread.Abort();

      this.RefreshButton.IsEnabled = false;
      Thread oThread = new Thread(() => this.PopulateListThread(this));
      this._activeUpdateThread = oThread;
      oThread.Start();
    }

    /// <summary>
    /// The thread that updates the list
    /// </summary>
    /// <param name="ovp">
    /// this overview page
    /// </param>
    public void PopulateListThread(OverviewPage ovp) {
      ovp.Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, 
        new Action(delegate { this.UpdateLabel.Content = "Scanning..."; }));
      ovp.Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, 
        new Action(delegate { this.LoadingBar.Visibility = Visibility.Visible; }));
      IEnumerable<IPEndPoint> peerlist = this._ui.GetPeerlist();

      if (peerlist != null) {
        var dataSource = (from ip in peerlist
                          where this._ui.IsStationActive(ip)
                          select new StationStatus { IpAddress = ip.Address.ToString(), ConnectionState = "Connected" }).ToList();
        IEnumerable<IPEndPoint> currentpeers = _ui.DiscoverPeers();
        Console.WriteLine(currentpeers.Count() + " peers discovered");
        dataSource.AddRange(
          from ip in currentpeers
          where !peerlist.Contains(ip)
          select new StationStatus { IpAddress = ip.Address.ToString(), ConnectionState = "Not Connected" });

        ovp.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.stationGrid.ItemsSource = dataSource; }));
        ovp.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate { this.stationGrid.Items.Refresh(); }));
      }

      ovp.Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate { this.UpdateLabel.Content = string.Empty; }));
      ovp.Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, 
        new Action(delegate { this.LoadingBar.Visibility = Visibility.Hidden; }));
      ovp.Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, 
        new Action(delegate { this.RefreshButton.IsEnabled = true; }));
      ovp.Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, 
        new Action(delegate { this.StartEndElectionButton.IsEnabled = true; }));
    }

    /// <summary>
    /// Sets the password label
    /// </summary>
    /// <param name="content">
    /// the string to set the label to
    /// </param>
    public void SetPasswordLabel(string content) { this.PasswordLabel.Content = content; }

    /// <summary>
    /// Unmark a connected station in the list
    /// </summary>
    /// <param name="ip">
    /// the IP address of the station to unmark
    /// </param>
    public void UnmarkSelectedStation(IPEndPoint ip) {
      foreach (StationStatus s in this.stationGrid.Items) {
        if (s.IpAddress == ip.Address.ToString()) {
          s.ConnectionState = "Not Connected";
        }
      }
      this.stationGrid.Items.Refresh();
    }

    public void SetSelectedStationStatus(IPEndPoint ip, string status) {
      foreach (StationStatus s in this.stationGrid.Items) {
        if (s.IpAddress == ip.Address.ToString()) {
          s.ConnectionState = status;
        }
      }
      this.stationGrid.Items.Refresh();
      UpdateControls();
    }

    public void MakeStationReady(IPEndPoint ip) {
      foreach (StationStatus s in this.stationGrid.Items) {
        if (s.IpAddress == ip.Address.ToString() && !s.Ready()) {
          s.ConnectionState = "Ready";
        }
      }
      this.stationGrid.Items.Refresh();
      UpdateControls();
    }

    public string GetSelectedStationStatus(IPEndPoint ip) {
      foreach (StationStatus s in this.stationGrid.Items) {
        if (s.IpAddress == ip.Address.ToString()) {
          return s.ConnectionState;
        }
      }
      return null;
    }

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
      if (this.stationGrid.SelectedCells.Count != 0) {
        this._ui.ExchangeKeys(
          new IPEndPoint(IPAddress.Parse(((StationStatus)this.stationGrid.SelectedItem).IpAddress), 62000));
        this.RemoveButton.IsEnabled = true;
      }
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
      if (this._activeUpdateThread != null) this._activeUpdateThread.Abort();

      this._parent.Navigate(new DataLoadPage(this._parent, this._ui));
      this._ui.DisposeStation();
      this._ui.OverviewPage = null;
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
    private void RefreshButtonClick(object sender, RoutedEventArgs e) { this.PopulateList(); UpdateControls(); }

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
      if (this.stationGrid.SelectedItem != null) {
        if (((StationStatus)this.stationGrid.SelectedItem).Connected()) {
          this._ui.RemoveStation(((StationStatus)this.stationGrid.SelectedItem).IpAddress);
          this.UnmarkSelectedStation(
            new IPEndPoint(IPAddress.Parse(((StationStatus)this.stationGrid.SelectedItem).IpAddress), 62000));
          this.PopulateList();
        }
      }
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
      if (!this._ui.EnoughStations()) {
        MessageBox.Show(
          "Du er ikke forbundet til nok stationer", 
          "Ikke nok stationer", 
          MessageBoxButton.OK, 
          MessageBoxImage.Information);
        return;
      }

      var d = new CheckMasterPasswordDialog(this._ui);
      d.ShowDialog();

      if (d.DialogResult.HasValue &&
          d.DialogResult == true) {
        if (d.IsCancel) return;

        if (this._activeUpdateThread != null) this._activeUpdateThread.Abort();

        this._ui.OverviewPage = null;
        this._ui.ManagerAnnounceStartElection();
        this._parent.Navigate(new ManagerOverviewPage(this._parent, this._ui));
      } else
        MessageBox.Show(
          "Det indtastede kodeord er ikke korrekt, prøv igen", "Forkert master kodeord", MessageBoxButton.OK);
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

    private void UpdateControls() {
      if ((this.stationGrid.SelectedItem != null) &&
          ((StationStatus)this.stationGrid.SelectedItem).Connected()) {
        this.AddButton.IsEnabled = false;
        this.RemoveButton.IsEnabled = true;
      } else {
        this.AddButton.IsEnabled = true;
        this.RemoveButton.IsEnabled = false;
      }

      StartEndElectionButton.IsEnabled = true;

      foreach (StationStatus s in stationGrid.ItemsSource) {
        if (s.ConnectionState.Equals("Synchronizing Election Data")) {
          StartEndElectionButton.IsEnabled = false;
        }
      }
    }

    #endregion
  }
}

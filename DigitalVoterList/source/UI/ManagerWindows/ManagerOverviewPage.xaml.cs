// --------------------------------------------------------------------------------------------------------------------
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
  using System.Windows.Controls;
  using System.Windows.Input;

  using Aegis_DVL.Database;

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
      this.InitializeComponent();
      this._parent = parent;
      this._ui = ui;
      this._ui.ManagerOverviewPage = this;

      this.LoadingBar.Visibility = Visibility.Hidden;
      this.LoadingBar.Value = 100;
      this.RemoveButton.IsEnabled = false;
      this.AddButton.IsEnabled = false;
      this.EndElectionButton.IsEnabled = false;

      // Change the width of the window
      var wnd = Window.GetWindow(this._parent);
      if (wnd != null) wnd.Width = 1000;

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
    /// Marks a machine as connected in the list
    /// </summary>
    /// <param name="ip">
    /// the ip address of the machine
    /// </param>
    public void MarkAsConnected(IPEndPoint ip) {
      foreach (StationStatus s in this.ManagerstationGrid.Items) {
        if (s.IpAddress == ip.Address.ToString()) {
          s.ConnectionState = "Connected";
        }
      }

      this.ManagerstationGrid.Items.Refresh();
    }

    public void SetSelectedStationStatus(IPEndPoint ip, string status) {
      foreach (StationStatus s in this.ManagerstationGrid.Items) {
        if (s.IpAddress == ip.Address.ToString()) {
          s.ConnectionState = status;
        }
      }

      this.ManagerstationGrid.Items.Refresh();
    }

    public void MakeStationReady(IPEndPoint ip) {
      foreach (StationStatus s in this.ManagerstationGrid.Items) {
        if (s.IpAddress == ip.Address.ToString() && !s.Ready()) {
          s.ConnectionState = "Ready";
        }
      }
      this.ManagerstationGrid.Items.Refresh();
      UpdateControls();
    }

    /// <summary>
    /// Populates the list with the appropiate machines
    /// </summary>
    public void PopulateList() {
      if (this._activeUpdateThread != null) return;
      this.EndElectionButton.IsEnabled = false;
      this.RefreshButton.IsEnabled = false;
      Thread oThread = new Thread(PopulateListThread);
      this._activeUpdateThread = oThread;
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
        new Action(delegate { this.UpdateLabel.Content = "Scanning..."; }));
      Dispatcher.Invoke(
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

        Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.ManagerstationGrid.ItemsSource = dataSource; }));
        Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.ManagerstationGrid.Items.Refresh(); }));
      }

      Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate { this.UpdateLabel.Content = string.Empty; }));
      Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, 
        new Action(delegate { this.LoadingBar.Visibility = Visibility.Hidden; }));
      Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, 
        new Action(delegate { this.RefreshButton.IsEnabled = true; }));
      Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, 
        new Action(delegate { this.EndElectionButton.IsEnabled = true; }));
      _activeUpdateThread = null;
    }

    /// <summary>
    /// Sets the password label
    /// </summary>
    /// <param name="content">
    /// the new content of the label
    /// </param>
    public void SetPasswordLabel(string content) { this.PasswordLabel.Content = content; }

    /// <summary>
    /// Unmark a connected station in the list
    /// </summary>
    /// <param name="ip">the IP address of the station to unmark</param>
    public void UnmarkSelectedStation() {
      ((StationStatus)this.ManagerstationGrid.SelectedItem).ConnectionState = "Not Connected";
      this.ManagerstationGrid.Items.Refresh();
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
      if (this.ManagerstationGrid.SelectedCells.Count != 0) {
        this._ui.ExchangeKeys(
          new IPEndPoint(IPAddress.Parse(((StationStatus)this.ManagerstationGrid.SelectedItem).IpAddress), 62000));
      }
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
      var d = new CheckMasterPasswordDialog(this._ui);
      d.ShowDialog();

      if (d.DialogResult.HasValue &&
          d.DialogResult == true) {
        if (d.IsCancel != true) {
          if (this._activeUpdateThread != null) this._activeUpdateThread.Abort();

          this._ui.AnnounceEndElection();
          this._ui.ManagerOverviewPage = null;
          this._parent.Navigate(new EndedElectionPage(this._parent, this._ui));
        }
      } else
          MessageBox.Show(
            "You have entered an incorrect master password, please try again.", "Incorrect Master Password", MessageBoxButton.OK);
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
      var d = new CheckMasterPasswordDialog(this._ui);
      d.ShowDialog();

      if (d.DialogResult.HasValue &&
          d.DialogResult == true) {
        if (this.ManagerstationGrid.SelectedItem != null &&
            this._ui.MakeManager(
              new IPEndPoint(IPAddress.Parse(((StationStatus)this.ManagerstationGrid.SelectedItem).IpAddress), 62000))) {
          if (d.IsCancel) return;

          if (this._activeUpdateThread != null) this._activeUpdateThread.Abort();

          var wnd = (StationWindow)Window.GetWindow(this._parent);
          if (wnd != null) wnd.Width = 600;

          this._ui.ManagerOverviewPage = null;
          this._parent.Navigate(new BallotRequestPage(this._ui, this._parent));
        } else {
          if (d.IsCancel) return;

          MessageBox.Show("Could not connect to the specified station", "No Connection", MessageBoxButton.OK);
        }
      } else {
        MessageBox.Show(
          "You have entered an incorrect master password, please try again.", 
          "Incorrect Master Password", 
          MessageBoxButton.OK, 
          MessageBoxImage.Stop);
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

    private void UpdateControls() {
      if (this.ManagerstationGrid.SelectedItem != null) {
        if (((StationStatus)this.ManagerstationGrid.SelectedItem).Connected()) {
          this.AddButton.IsEnabled = false;
          this.RemoveButton.IsEnabled = true;
        } else {
          this.AddButton.IsEnabled = true;
          this.RemoveButton.IsEnabled = false;
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
    private void RefreshButtonClick(object sender, RoutedEventArgs e) { this.PopulateList(); }

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
      if (((StationStatus)this.ManagerstationGrid.SelectedItem).Connected()) {
        this._ui.RemoveStation(((StationStatus)this.ManagerstationGrid.SelectedItem).IpAddress);
        this.UnmarkSelectedStation();
        this.PopulateList();
      }
    }

    #endregion
  }
}

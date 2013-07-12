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

      this.checkValidityButton.IsEnabled = false;

      this.WaitingLabel.Content = string.Empty;
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
    /// Called when a response to a ballot request is received
    /// </summary>
    /// <param name="succes">
    /// whether or not to hand out a ballot
    /// </param>
    public void BallotResponse(bool succes) {
      this.WaitingLabel.Content = string.Empty;

      if (succes) {
        MessageBox.Show(
          "Vælgeren " + this.voterCardNumberTextbox.Text + " må gives en stemmeseddel ", 
          "Giv stemmeseddel", 
          MessageBoxButton.OK, 
          MessageBoxImage.Exclamation);
      } else {
        MessageBox.Show(
          "Vælgeren " + this.voterCardNumberTextbox.Text + " må IKKE gives en stemmeseddel ", 
          "Giv ikke stemmeseddel", 
          MessageBoxButton.OK, 
          MessageBoxImage.Stop);
      }

      this.voterCardNumberTextbox.Text = string.Empty;

      this.CPRNumberTextbox.Text = string.Empty;
    }

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
      foreach (StationStatus s in this.ManagerstationGrid.Items) if (s.IpAdress == ip.Address.ToString()) s.Connected = true;
      this.ManagerstationGrid.Items.Refresh();
    }

    /// <summary>
    /// Populates the list with the appropiate machines
    /// </summary>
    public void PopulateList() {
      this.EndElectionButton.IsEnabled = false;

      if (this._activeUpdateThread != null) this._activeUpdateThread.Abort();

      this.RefreshButton.IsEnabled = false;
      Thread oThread = new Thread(() => this.PopulateListThread(this));
      this._activeUpdateThread = oThread;
      oThread.Start();
    }

    /// <summary>
    /// the thread to populate the list
    /// </summary>
    /// <param name="mvp">
    /// this manager overview page
    /// </param>
    public void PopulateListThread(ManagerOverviewPage mvp) {
      mvp.Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, 
        new Action(delegate { this.UpdateLabel.Content = "Opdaterer..."; }));
      mvp.Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, 
        new Action(delegate { this.LoadingBar.Visibility = Visibility.Visible; }));
      IEnumerable<IPEndPoint> peerlist = this._ui.GetPeerlist();

      if (peerlist != null) {
        var dataSource = (from ip in peerlist
                          where this._ui.IsStationActive(ip)
                          select new StationStatus { IpAdress = ip.Address.ToString(), Connected = true }).ToList();
        dataSource.AddRange(
          from ip in this._ui.DiscoverPeers()
          where !peerlist.Contains(ip)
          select new StationStatus { IpAdress = ip.Address.ToString(), Connected = false });

        mvp.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.ManagerstationGrid.ItemsSource = dataSource; }));
        mvp.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.ManagerstationGrid.Items.Refresh(); }));
      }

      mvp.Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate { this.UpdateLabel.Content = string.Empty; }));
      mvp.Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, 
        new Action(delegate { this.LoadingBar.Visibility = Visibility.Hidden; }));
      mvp.Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, 
        new Action(delegate { this.RefreshButton.IsEnabled = true; }));
      mvp.Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, 
        new Action(delegate { this.EndElectionButton.IsEnabled = true; }));
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
      ((StationStatus)this.ManagerstationGrid.SelectedItem).Connected = false;
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
          new IPEndPoint(IPAddress.Parse(((StationStatus)this.ManagerstationGrid.SelectedItem).IpAdress), 62000));
      }
    }

    /// <summary>
    /// Called when the CPRNumberTextbox text changes
    /// </summary>
    /// <param name="sender">
    /// autogenerated
    /// </param>
    /// <param name="e">
    /// autogenerated
    /// </param>
    private void CPRNumberTextboxTextChanged(object sender, TextChangedEventArgs e) {
      if (this.CPRNumberTextbox.Text.Length > 0) this.voterCardNumberTextbox.Text = string.Empty;
      else this.CPRNumberTextbox.Text = string.Empty;

      if ((this.CPRNumberTextbox.Text.Length == 10 && this.WaitingLabel.Content.Equals(string.Empty) && !this.Blocked) ||
          (this.voterCardNumberTextbox.Text.Length == 6 /*TODO: Correct to voternumber length*/&&
           this.WaitingLabel.Content.Equals(string.Empty) && !this.Blocked)) this.checkValidityButton.IsEnabled = true;
      else this.checkValidityButton.IsEnabled = false;
    }

    /// <summary>
    /// Called when the done button is clicked
    /// </summary>
    /// <param name="sender">
    /// autogenerated
    /// </param>
    /// <param name="e">
    /// autogenerated
    /// </param>
    private void CheckValidityButtonClick(object sender, RoutedEventArgs e) {
      if (!this.voterCardNumberTextbox.Text.Equals(string.Empty)) {
        this.WaitingLabel.Content = "Venter på svar..";
        this._ui.RequestBallot(this.voterCardNumberTextbox.Text);
      } else {
        if (!this.CPRNumberTextbox.Text.Equals(string.Empty)) {
          var d = new CheckMasterPasswordDialog(this._ui);
          d.ShowDialog();

          if (d.DialogResult.HasValue &&
              d.DialogResult == true) {
            if (d.IsCancel) return;

            this._ui.RequestBallotOnlyCPR(this.CPRNumberTextbox.Text, d.TypedPassword);
          } else
            MessageBox.Show(
              "Det kodeord du indtastede er ikke korret, prøv igen", "Forkert Master Kodeord", MessageBoxButton.OK);
        }
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
          "Det indtastede kodeord er ikke korret, prøv igen", "Forkert Master Kodeord", MessageBoxButton.OK);
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
              new IPEndPoint(IPAddress.Parse(((StationStatus)this.ManagerstationGrid.SelectedItem).IpAdress), 62000))) {
          if (d.IsCancel) return;

          if (this._activeUpdateThread != null) this._activeUpdateThread.Abort();

          var wnd = (StationWindow)Window.GetWindow(this._parent);
          if (wnd != null) wnd.Width = 600;

          this._ui.ManagerOverviewPage = null;
          this._parent.Navigate(new BallotRequestPage(this._ui, this._parent));
        } else {
          if (d.IsCancel) return;

          MessageBox.Show("Der kunne ikke forbindes til den valgte station", "Ingen forbindelse", MessageBoxButton.OK);
        }
      } else {
        MessageBox.Show(
          "Det kodeord du indtastede er ikke korrekt, prøv igen", 
          "Forkert Master Kodeord", 
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
      if (this.ManagerstationGrid.SelectedItem != null) {
        if (((StationStatus)this.ManagerstationGrid.SelectedItem).Connected) {
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
      if (((StationStatus)this.ManagerstationGrid.SelectedItem).Connected) {
        this._ui.RemoveStation(((StationStatus)this.ManagerstationGrid.SelectedItem).IpAdress);
        this.UnmarkSelectedStation();
        this.PopulateList();
      }
    }

    #endregion
  }
}

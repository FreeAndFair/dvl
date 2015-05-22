// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaitingForManagerPage.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for WaitingForManagerPage.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="WaitingForManagerPage.xaml.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace UI.StationWindows {
  using System;
  using System.Net;
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  /// Interaction logic for WaitingForManagerPage.xaml
  /// </summary>
  public partial class WaitingForManagerPage {
    #region Fields

    /// <summary>
    /// The typed password.
    /// </summary>
    public string TypedPassword;

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
    /// Initializes a new instance of the <see cref="WaitingForManagerPage"/> class.
    /// </summary>
    /// <param name="parent">
    /// The parent.
    /// </param>
    /// <param name="ui">
    /// The ui.
    /// </param>
    public WaitingForManagerPage(Frame parent, UiHandler ui) {
      this._parent = parent;
      this._ui = ui;
      this.InitializeComponent();
      Window.GetWindow(this._parent);
      this.IPLabel.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
        new Action(delegate { this.IPLabel.Content = "This is station " + _ui.IPAddressString; }));
      this._ui.WaitingForManagerPage = this;
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// This will be called when a manager wants to connect to this station.
    /// Accessable via UIHandler.ManagerExchangingKey(IPEndPoint ip)
    /// </summary>
    /// <param name="ip">
    /// the ip adress of the manager
    /// </param>
    /// <returns>
    /// the password typed on the station
    /// </returns>
    public string IncomingConnection(IPEndPoint ip) {
      var amd = new AcceptManagerDialog(this._parent, ip, this);
      var result = amd.ShowDialog();

      if (result.HasValue &&
          result == true) {
        this.CenterLabel.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.CenterLabel.Content = "Waiting for election data..."; }));
        return this.TypedPassword;
      }

      return string.Empty;
    }

    /// <summary>
    /// The set password label.
    /// </summary>
    /// <param name="content">
    /// The content.
    /// </param>
    public void SetPasswordLabel(string content) { this.PasswordLabel.Content = content; }

    /// <summary>
    /// When the manager has connected, entered the password and the station has done the same, we navigate to the next screen.
    /// </summary>
    public void StartElection() {
      this._ui.WaitingForManagerPage = null;
      this._parent.Navigate(new BallotRequestPage(this._ui, this._parent));
    }

    /// <summary>
    /// When the station is told that it has been removed, we navigate to the TypeChoicePage.
    /// </summary>
    public void StationRemoved() {
      this._ui.WaitingForManagerPage = null;
      this._ui.DisposeStation();
      this._parent.Navigate(new TypeChoicePage(this._parent, this._ui));
    }

    #endregion

    #region Methods

    /// <summary>
    /// The back button click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void BackButtonClick(object sender, RoutedEventArgs e) {
      this._ui.WaitingForManagerPage = null;
      this._ui.DisposeStation();
      this._parent.Navigate(new TypeChoicePage(this._parent, this._ui));
    }

    #endregion
  }
}

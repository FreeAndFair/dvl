// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeChoicePage.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for TypeChoicePage.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="TypeChoicePage.xaml.cs" company="DemTech">
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

  using UI.ManagerWindows;
  using UI.StationWindows;

  /// <summary>
  /// Interaction logic for TypeChoicePage.xaml
  /// </summary>
  public partial class TypeChoicePage {
    #region Fields

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
    /// Initializes a new instance of the <see cref="TypeChoicePage"/> class. 
    /// Constructor
    /// </summary>
    /// <param name="parent">
    /// the frame in which this page is displayed
    /// </param>
    /// <param name="ui">
    /// the UIhandler of this UI
    /// </param>
    public TypeChoicePage(Frame parent, UiHandler ui) {
      this._parent = parent;
      this._ui = ui;
      this.InitializeComponent();
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The remove station.
    /// </summary>
    public void RemoveStation() {
      this._ui.BallotRequestPage = null;
      this._ui.WaitingForManagerPage = null;
      this._ui.WaitingForManagerPage = new WaitingForManagerPage(this._parent, this._ui);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Called when the exit button is pressed
    /// </summary>
    /// <param name="sender">
    /// auto generated
    /// </param>
    /// <param name="e">
    /// auto generated
    /// </param>
    private void ExitButtonClick(object sender, RoutedEventArgs e) { Environment.Exit(0); }

    /// <summary>
    /// Called when the manager option is chosen
    /// </summary>
    /// <param name="sender">
    /// auto generated
    /// </param>
    /// <param name="e">
    /// auto generated
    /// </param>
    private void ManagerButtonClick(object sender, RoutedEventArgs e) { this._parent.Navigate(new MasterPasswordPage(this._parent, this._ui)); }

    /// <summary>
    /// Called when the station option is chosen
    /// </summary>
    /// <param name="sender">
    /// auto generated
    /// </param>
    /// <param name="e">
    /// auto generated
    /// </param>
    private void StationButtonClick(object sender, RoutedEventArgs e) {
      this._ui.CreateNewStation();
      this._parent.Navigate(new WaitingForManagerPage(this._parent, this._ui));
    }

    #endregion
  }
}

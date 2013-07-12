// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Settings.cs" company="">
//   
// </copyright>
// <summary>
//   The settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="Settings.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace UI.Properties {
  using System.ComponentModel;
  using System.Configuration;

  // This class allows you to handle specific events on the settings class:
  // The SettingChanging event is raised before a setting's value is changed.
  // The PropertyChanged event is raised after a setting's value is changed.
  // The SettingsLoaded event is raised after the setting values are loaded.
  // The SettingsSaving event is raised before the setting values are saved.
  /// <summary>
  /// The settings.
  /// </summary>
  internal sealed partial class Settings {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Settings"/> class.
    /// </summary>
    public Settings() {
      // // To add event handlers for saving and changing settings, uncomment the lines below:
      // this.SettingChanging += this.SettingChangingEventHandler;
      // this.SettingsSaving += this.SettingsSavingEventHandler;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The setting changing event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void SettingChangingEventHandler(object sender, SettingChangingEventArgs e) {
      // Add code to handle the SettingChangingEvent event here.
    }

    /// <summary>
    /// The settings saving event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void SettingsSavingEventHandler(object sender, CancelEventArgs e) {
      // Add code to handle the SettingsSaving event here.
    }

    #endregion
  }
}

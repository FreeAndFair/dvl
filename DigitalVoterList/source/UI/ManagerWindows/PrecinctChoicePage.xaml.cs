namespace UI.ManagerWindows {
  /// <summary>
  /// Interaction logic for PrecinctChoicePage.xaml
  /// </summary>
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net;
  using System.Threading;
  using System.Windows;
  using System.Windows.Controls;

  using Aegis_DVL.Database;
  using Aegis_DVL.Data_Types;
  using UI.Data;

  public partial class PrecinctChoicePage : Page {
    private readonly Frame _parent;
    private readonly UiHandler _ui;
    private readonly SortedDictionary<string, PollingPlace> _places;

    public PrecinctChoicePage(Frame parent, UiHandler ui) {
      InitializeComponent();
      _parent = parent;
      _ui = ui;
      _ui.PrecinctChoicePage = this;
      _places = new SortedDictionary<string, PollingPlace>();
      PopulateList();
    }

    private void PopulateList() {
      foreach (Precinct p in _ui._station.Database.AllPrecincts) {
        if (p.LocationName.Trim().Length > 0) {
          if (_places.ContainsKey(p.LocationName)) {
            _places[p.LocationName].PrecinctIds.Add(p.PrecinctSplitId);
          } else {
            _places[p.LocationName] = new PollingPlace(p);
          }
        }
      }
      Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal,
        new Action(delegate { PrecinctGrid.ItemsSource = _places.Values; }));
      Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate { PrecinctGrid.Items.Refresh(); }));
    }

    private void SelectClick(object sender, RoutedEventArgs e) {
      this._ui._station.PollingPlace = (PollingPlace) PrecinctGrid.SelectedItem;
      this._parent.Navigate(new OverviewPage(this._parent, this._ui));
    }

    private void BackClick(object sender, RoutedEventArgs e) {
      this._parent.Navigate(new TypeChoicePage(this._parent, this._ui));
      this._ui.DisposeStation();
    }

    private void PrecinctGridSelectionChanged(object sender, SelectionChangedEventArgs e) {
      this.SelectButton.IsEnabled = (this.PrecinctGrid.SelectedItem != null);
    }
  }
}

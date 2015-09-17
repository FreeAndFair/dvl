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
      _ui._station.PollingPlace = (PollingPlace) PrecinctGrid.SelectedItem;
      _parent.Navigate(new OverviewPage(_parent, _ui));
    }

    private void VoteCenterClick(object sender, RoutedEventArgs e) {
      _ui._station.PollingPlace = new PollingPlace();
      HashSet<string> allPrecincts = new HashSet<string>();
      foreach (Precinct p in _ui._station.Database.AllPrecincts) {
        allPrecincts.Add(p.PrecinctSplitId);
      }
      foreach (string s in allPrecincts) {
        _ui._station.PollingPlace.PrecinctIds.Add(s);
      }
      _parent.Navigate(new OverviewPage(_parent, _ui));
    }

    private void BackClick(object sender, RoutedEventArgs e) {
      _parent.Navigate(new TypeChoicePage(_parent, _ui));
      _ui.DisposeStation();
    }

    private void PrecinctGridSelectionChanged(object sender, SelectionChangedEventArgs e) {
      SelectButton.IsEnabled = (PrecinctGrid.SelectedItem != null);
    }
  }
}

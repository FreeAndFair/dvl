using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Aegis_DVL.Database;
using UI.Data;

namespace UI.StationWindows {
  /// <summary>
  /// Interaction logic for ConfirmMultiVoterDialog.xaml
  /// </summary>
  public partial class ConfirmMultiVoterDialog : Window {
    private readonly List<VoterListing> _voterlistings;
    private readonly List<Voter> _voters;
    
    public Voter SelectedVoter { get; set; }

    public ConfirmMultiVoterDialog(List<Voter> voters) {
      InitializeComponent();
      _voters = voters;
      _voterlistings = new List<VoterListing>();
      Count.Content = Count.Content.ToString().Replace("99999", _voters.Count.ToString());
      PopulateList();
      OKButton.IsEnabled = false;
    }

    private void PopulateList() {
      foreach (Voter v in _voters) {
        VoterListing vl = new VoterListing();
        string votername;
        string middlename = "";
        if (v.MiddleName != null && v.MiddleName.Trim().Length != 0) {
          middlename = " " + v.MiddleName;
        }
        votername = v.LastName + ", " + v.FirstName + middlename;
        if (v.Suffix != null && v.Suffix.Trim().Length > 0) {
          votername = votername + ", " + v.Suffix;
        }
        vl.Name = votername;
        if (v.ProtectedAddress) {
          vl.Address = "Address Protected for Privacy";
        } else {
          vl.Address = v.Address + ", " + v.Municipality;
        }
        vl.DateOfBirth = v.DateOfBirth.Date.ToString("MM/dd/yyyy");
        if (v.DriversLicense.Length > 0) {
          vl.DriversLicense = v.DriversLicense;
        } else {
          vl.DriversLicense = "Not On File";
        }
        if (v.StateId != null) {
          vl.VUID = v.StateId.ToString();
        } else {
          vl.VUID = "Not On File";
        }
        _voterlistings.Add(vl);
      }
      Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal,
          new Action(delegate { VoterGrid.ItemsSource = _voterlistings; }));
      Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate { VoterGrid.Items.Refresh(); }));

    }

    private void OKClick(object sender, RoutedEventArgs e) {
      if (VoterGrid.SelectedItem == null) {
        return;
      }
      DialogResult = true;
      int i = _voterlistings.IndexOf((VoterListing)VoterGrid.SelectedItem);
      SelectedVoter = _voters[i];
      Close();
    }

    private void CancelClick(object sender, RoutedEventArgs e) {
      DialogResult = false;
      Close();
    }

    private void GridSelectionChanged(object sender, SelectionChangedEventArgs e) {
      OKButton.IsEnabled = VoterGrid.SelectedItem != null;
    }
  }
}

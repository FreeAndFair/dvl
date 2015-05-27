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

namespace UI.StationWindows {
  /// <summary>
  /// Interaction logic for ConfirmSingleVoterDialog.xaml
  /// </summary>
  public partial class ConfirmSingleVoterDialog : Window {
    public ConfirmSingleVoterDialog(Voter v) {
      InitializeComponent();

      string votername;
      string middlename = " ";
      if (v.MiddleName != null && v.MiddleName.Trim().Length != 0) {
        middlename = " " + v.MiddleName + " ";
      }
      votername = v.FirstName + middlename + v.LastName;
      if (v.Suffix != null && v.Suffix.Trim().Length > 0) {
        votername = votername + ", " + v.Suffix;
      }
      NameBox.Content = votername;
      if (v.ProtectedAddress) {
        Address.Content = "Address Protected for Privacy";
        MunicipalityAndZIP.Content = "";
      } else {
        Address.Content = v.Address;
        MunicipalityAndZIP.Content = v.Municipality + "  " + v.ZipCode;
      }
      VUID.Content = "VUID: " + v.StateId;
      DateOfBirth.Content = "Date Of Birth: " + v.DateOfBirth.Date.ToString("MM/dd/yyyy");
      if (v.DriversLicense.Length > 0) {
        DriversLicense.Content = "Drivers License: " + v.DriversLicense;
      } else {
        DriversLicense.Content = "Drivers License: Not On File";
      }
    }

    private void OKClick(object sender, RoutedEventArgs e) {
      DialogResult = true;
      Close();
    }

    private void CancelClick(object sender, RoutedEventArgs e) {
      DialogResult = false;
      Close();
    }
  }
}

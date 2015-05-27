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
using Aegis_DVL.Data_Types;

namespace UI.StationWindows {
  /// <summary>
  /// Interaction logic for ConfirmSingleVoterWithConditionsDialog.xaml
  /// </summary>
  public partial class ConfirmSingleVoterWithConditionsDialog : Window {
    public ConfirmSingleVoterWithConditionsDialog(Voter v, VoterStatus vs) {
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
        MunicipalityAndZIP.Content = v.Municipality + ", TX  " + v.ZipCode;
      }
      DateOfBirth.Content = "Date Of Birth: " + v.DateOfBirth.Date.ToString("MM/dd/yyyy");
      VUID.Content = "VUID: " + v.StateId;
      if (v.DriversLicense.Length > 0) {
        DriversLicense.Content = "Drivers License: " + v.DriversLicense;
      } else {
        DriversLicense.Content = "Drivers License: Not On File";
      }

      VoterConditions.Text = GetConditions(vs);
      OKButton.IsEnabled = false;
    }

    private string GetConditions(VoterStatus vs) {
      string result;
      switch (vs) {
        case VoterStatus.SuspendedVoter:
          result = "This voter is a suspense voter. Please obtain an affidavit of residence before you complete the check in.";
          break;
        case VoterStatus.MailBallotNotReturned:
          result = "This voter was sent an absentee ballot and has not yet returned it. Please obtain an " +
            "affidavit before you complete the check in.";
          break;
        case VoterStatus.OutOfCounty:
          result = "This voter is an out of county voter. Please obtain appropriate affidavits " +
            "before you complete the check in.";
          break;
        default:
          result = "This dialog box is being shown inappropriately. Please consult the developers.";
          break;
      }
      return result;
    }

    private void OKClick(object sender, RoutedEventArgs e) {
      DialogResult = true;
      Close();
    }

    private void CancelClick(object sender, RoutedEventArgs e) {
      DialogResult = false;
      Close();
    }

    private void AffidavitsClick(object sender, RoutedEventArgs e) {
      OKButton.IsEnabled = (bool) Affidavits.IsChecked;
    }
  }
}

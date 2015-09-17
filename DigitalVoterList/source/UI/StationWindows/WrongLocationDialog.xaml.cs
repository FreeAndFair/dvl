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
  /// Interaction logic for WrongLocationDialog.xaml
  /// </summary>
  public partial class WrongLocationDialog : Window {
    public WrongLocationDialog(Voter voter, Precinct place) {
      InitializeComponent();
      this.Owner = Application.Current.MainWindow;
      VoterName.Content = GetFormattedName(voter);
      StringBuilder sb = new StringBuilder();
      sb.Append(place.Address);
      sb.Append("\n");
      sb.Append(place.CityStateZIP);
      LocationAddress.Text = sb.ToString();
    }

    private void ContinueClick(object sender, RoutedEventArgs e) {
      DialogResult = false;
      Close();
    }

    private void DirectionsClick(object sender, RoutedEventArgs e) {
      DialogResult = true;
      Close();
    }

    private string GetFormattedName(Voter voter) {
      string votername;
      string middlename = " ";
      if (voter.MiddleName != null && voter.MiddleName.Trim().Length != 0) {
        middlename = " " + voter.MiddleName + " ";
      }
      votername = voter.FirstName + middlename + voter.LastName;

      if (voter.Suffix != null && voter.Suffix.Trim().Length > 0) {
        votername = votername + ", " + voter.Suffix;
      }
      return votername;
    }
  }
}

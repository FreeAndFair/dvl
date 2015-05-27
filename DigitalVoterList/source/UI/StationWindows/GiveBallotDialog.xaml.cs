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
  /// Interaction logic for GiveBallotDialog.xaml
  /// </summary>
  public partial class GiveBallotDialog : Window {
    public GiveBallotDialog(Voter voter) {
      InitializeComponent();
      VoterName.Content = GetFormattedName(voter);
      Precinct.Text = Precinct.Text.Replace("PSUB", voter.PrecinctSub);
      BallotStyle.Text = voter.BallotStyle.ToString();
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

    private void ContinueClick(object sender, RoutedEventArgs e) {
      DialogResult = true;
      Close();
    }
  }
}

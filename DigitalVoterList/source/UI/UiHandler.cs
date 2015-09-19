#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="UiHandler.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace UI {
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Net.Sockets;
  using System.Text;
  using System.Windows.Forms;
  using System.Windows.Interop;

  using Aegis_DVL;
  using Aegis_DVL.Commands;
  using Aegis_DVL.Cryptography;
  using Aegis_DVL.Data_Types;
  using Aegis_DVL.Database;
  using Aegis_DVL.Util;

  using UI.ManagerWindows;
  using UI.StationWindows;

  /// <summary>
  /// The ui handler.
  /// </summary>
  public class UiHandler : IDvlUi {
    #region Fields

    /// <summary>
    /// The ballot request page.
    /// </summary>
    public BallotRequestPage BallotRequestPage;

    /// <summary>
    /// The manager overview page.
    /// </summary>
    public ManagerOverviewPage ManagerOverviewPage;

    /// <summary>
    /// The overview page.
    /// </summary>
    public OverviewPage OverviewPage;

    public PrecinctChoicePage PrecinctChoicePage;

    /// <summary>
    /// The waiting for manager page.
    /// </summary>
    public WaitingForManagerPage WaitingForManagerPage;

    /// <summary>
    /// The _station window.
    /// </summary>
    public readonly StationWindow _stationWindow;

    /// <summary>
    /// The _station window as a NativeWindow.
    /// </summary>
    public readonly NativeWindow _stationNativeWindow;

    /// <summary>
    /// The _master password.
    /// </summary>
    private string _masterPassword;

    /// <summary>
    /// The _station.
    /// </summary>
    public Station _station;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="UiHandler"/> class.
    /// </summary>
    /// <param name="stationWindow">
    /// The station window.
    /// </param>
    public UiHandler(StationWindow stationWindow) { 
      _stationWindow = stationWindow;
      _stationNativeWindow = new NativeWindow();
      _stationNativeWindow.AssignHandle(new WindowInteropHelper(_stationWindow).EnsureHandle());
      if (_stationNativeWindow.Handle == IntPtr.Zero) {
        Console.WriteLine("STATION NATIVE WINDOW HAS INT PTR ZERO!");
      }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// When a manager want to end the election this method should be called
    /// to announce the end to all stations.
    /// </summary>
    public void AnnounceEndElection() { _station.AnnounceEndElection(); }

    public void BallotRequestReply(VoterNumber vn, bool success, VoterStatus oldStatus, VoterStatus newStatus) {
      BallotRequestReply(_station.Database.GetVoterByVoterNumber(vn), success, oldStatus, newStatus);
    }

    /// <summary>
    /// The ballot request reply.
    /// </summary>
    /// <param name="successful">
    /// The hand out ballot.
    /// </param>
    public void BallotRequestReply(Voter voter, bool success, VoterStatus oldStatus, VoterStatus newStatus) {
      if (BallotRequestPage != null) {
        BallotRequestPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { BallotRequestPage.BallotResponse(voter, success, oldStatus, newStatus); }));
      }
    }

    /// <summary>
    /// Creates a new station and makes it start listening
    /// </summary>
    public void CreateNewStation() { _station = new Station(this); }

    /// <summary>
    /// Gets he IP adresses of the machines in the local network running this application
    /// </summary>
    /// <returns>a list of IP adresses</returns>
    public void DiscoverPeers() { _station.DiscoverPeers(); }

    /// <summary>
    /// Closes the current station (in response to window closure or close button or quit).
    /// </summary>
    public void CloseStation() {
      _station.RemoveSelf();
    }

    /// <summary>
    /// disposes the current station
    /// </summary>
    public void DisposeStation() {
      if (_station != null) {
        _station.Dispose();
      }
      _station = null;
    }

    /// <summary>
    /// The election ended.
    /// </summary>
    public void ElectionEnded() {
      if (BallotRequestPage != null) {
        BallotRequestPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { BallotRequestPage.EndElection(); }));
      }
    }

    /// <summary>
    /// The election started.
    /// </summary>
    public void ElectionStarted() {
      if (WaitingForManagerPage != null) {
        WaitingForManagerPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { WaitingForManagerPage.StartElection(); }));
      }
    }

    /// <summary>
    /// The enough peers.
    /// </summary>
    public void EnoughPeers() {
      if (BallotRequestPage != null) {
        BallotRequestPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { BallotRequestPage.Blocked = false;
          BallotRequestPage.WaitingLabel.Content = ""; 
        }));
      }

      if (ManagerOverviewPage != null) {
        ManagerOverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { ManagerOverviewPage.Blocked = false; }));
      }
    }

    /// <summary>
    /// Checks if there are enough stations in the peerlist
    /// </summary>
    /// <returns>wheter there are enough stations is hte peerlist</returns>
    public bool EnoughStations() { return _station.EnoughStations; }

    /// <summary>
    /// When a manager wants to connect to a station, this is called to exchange the public keys
    /// </summary>
    /// <param name="ip">
    /// The IP address of the station
    /// </param>
    public void ExchangeKeys(IPEndPoint ip) { _station.ExchangePublicKeys(ip); }

    /// <summary>
    /// Export data in the format IEnumerable of EncryptedVoterData to a file
    /// </summary>
    /// <param name="data">
    /// the voter data to be exported
    /// </param>
    /// <param name="filePath">
    /// the file destination
    /// </param>
    public void ExportData(IEnumerable<Voter> voters, string filePath) {
      try {
        using (StreamWriter file = new StreamWriter(filePath)) {
          file.WriteLine("Election Report - Generated at " + DateTime.Now);
          file.WriteLine();
          file.WriteLine("Summary Information");
          file.WriteLine("-------------------");
          file.WriteLine();
          file.WriteLine("Known Voters: " + voters.Count());
          file.WriteLine("Registered to Vote Here: " + voters.Where(voter => _station.PollingPlace.PrecinctIds.Contains(voter.PrecinctSub)).Count());
          file.WriteLine("Checked In: " + voters.Where(voter => voter.PollbookStatus != (int)VoterStatus.NotSeenToday).Count());
          file.WriteLine();
          file.WriteLine("Active: " + voters.Where(voter => voter.PollbookStatus == (int)VoterStatus.ActiveVoter).Count());
          file.WriteLine("Suspended: " + voters.Where(voter => voter.PollbookStatus == (int)VoterStatus.SuspendedVoter).Count());
          file.WriteLine("Out of County: " + voters.Where(voter => voter.PollbookStatus == (int)VoterStatus.OutOfCounty).Count());
          file.WriteLine("Wrong Location: " + voters.Where(voter => voter.PollbookStatus == (int)VoterStatus.WrongLocation).Count());
          file.WriteLine("Already Voted Early: " + voters.Where(voter => voter.PollbookStatus == (int)VoterStatus.EarlyVotedInPerson).Count());
          file.WriteLine("Already Voted Absentee: " + voters.Where(voter => voter.PollbookStatus == (int)VoterStatus.VotedByMail).Count());
          file.WriteLine("Mail Ballot Not Returned: " + voters.Where(voter => voter.PollbookStatus == (int)VoterStatus.MailBallotNotReturned).Count());
          file.WriteLine("Otherwise Ineligible: " + voters.Where(voter => voter.PollbookStatus == (int)VoterStatus.Ineligible).Count());
          file.WriteLine();
          file.WriteLine("Voter Information");
          file.WriteLine("-----------------");
          file.WriteLine();
          file.WriteLine("This list contains all the voters that have checked in as of this time, and their statuses.");
          file.WriteLine("IMPORTANT: in the actual product, this output would be encrypted to protect voter privacy!");
          file.WriteLine();
          foreach (Voter v in voters) {
            if (v.PollbookStatus != (int)VoterStatus.NotSeenToday) {
              StringBuilder sb = new StringBuilder();
              string votername;
              string middlename = " ";
              if (v.MiddleName != null && v.MiddleName.Trim().Length != 0) {
                middlename = " " + v.MiddleName + " ";
              }
              votername = v.FirstName + middlename + v.LastName;
              if (v.Suffix != null && v.Suffix.Trim().Length > 0) {
                votername = votername + ", " + v.Suffix;
              }
              sb.Append(votername);
              sb.Append(", ");
              if (v.ProtectedAddress) {
                sb.Append("Address Protected for Privacy, ");
              } else {
                sb.Append(v.Address + ", " + v.Municipality + " " + v.ZipCode + ", ");
              }
              sb.Append("VUID " + v.StateId + ", ");
              sb.Append("DOB " + v.DateOfBirth.Date.ToString("MM/dd/yyyy") + ", ");
              if (v.DriversLicense.Length > 0) {
                sb.Append("DL " + v.DriversLicense + ", Status: ");
              } else {
                sb.Append("DL Not On File, Status: ");
              }
              switch ((VoterStatus)v.PollbookStatus) {
                case VoterStatus.ActiveVoter:
                  sb.Append("Active"); break;
                case VoterStatus.SuspendedVoter:
                  sb.Append("Suspense"); break;
                case VoterStatus.MailBallotNotReturned:
                  sb.Append("Didn't Return Mailed Ballot"); break;
                case VoterStatus.OutOfCounty:
                  sb.Append("Out of County"); break;
                case VoterStatus.Provisional:
                  sb.Append("Provisional"); break;
                case VoterStatus.VotedByMail:
                  sb.Append("Voted by Mail"); break;
                case VoterStatus.EarlyVotedInPerson:
                  sb.Append("Voted Early In-Person"); break;
                case VoterStatus.WrongLocation:
                  sb.Append("Wrong Location"); break;
                case VoterStatus.AbsenteeVotedInPerson:
                  sb.Append("Absentee Voted In Person"); break;
                case VoterStatus.Ineligible:
                  sb.Append("Ineligible"); break;
                default:
                  sb.Append("Unknown"); break;
              }
              file.WriteLine(sb.ToString());
            }
          }
          file.WriteLine();
          file.WriteLine("END OF REPORT");
        }
      } catch (Exception) {
      }
    }

    /// <summary>
    /// When a manager wants to export voter data this is called
    /// </summary>
    /// <param name="filePath">
    /// the destination filepath
    /// </param>
    public void ExportData(string filePath) {
      if (_station != null) ExportData(_station.Database.AllVoters, filePath);
      else FlexibleMessageBox.Show(_stationNativeWindow, "You can not export data at this time.", "Operation Not Allowed", MessageBoxButtons.OK);
    }

    /// <summary>
    /// This methods asks the station for a password it can use as master password
    /// </summary>
    /// <returns>the master password</returns>
    public string GeneratePassword() {
      _masterPassword = Crypto.GeneratePassword();
      return _masterPassword;
    }

    /// <summary>
    /// Asks the _station for the list of peers as IPEndpoints
    /// </summary>
    /// <returns>the list of peers as IPEndpoints</returns>
    public IEnumerable<IPEndPoint> GetPeerlist() { return _station.Peers.Keys; }

    /// <summary>
    /// When a manager wants to import voter data this is called
    /// </summary>
    /// <param name="voterDataPath">
    /// the file path of the data to be imported
    /// </param>
    /// <param name="precinctDataPath">
    /// the file path of the encryption key
    /// </param>
    /// <returns>
    /// whether or not the import was succesful
    /// </returns>
    public bool ImportData(string voterDataPath, string precinctDataPath) {
      AsymmetricKey key = new AsymmetricKey(
        KeyUtil.ToKey(new byte[] {0x30, 0x81, 0x9F, 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 
          0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00, 0x03, 0x81, 0x8D, 0x00, 0x30, 0x81, 
          0x89, 0x02, 0x81, 0x81, 0x00, 0xA4, 0x1A, 0xB3, 0xC6, 0x86, 0x85, 0x5D, 0xF4, 0xBC, 
          0x59, 0xC0, 0xBE, 0x89, 0xC4, 0x32, 0xA3, 0x9C, 0x21, 0xEC, 0x4D, 0x4D, 0xD3, 0x2C, 
          0x53, 0x0E, 0xEA, 0xAB, 0xA5, 0x9A, 0x96, 0xCA, 0x1C, 0xB2, 0xE3, 0xBF, 0x70, 0x52, 
          0x99, 0xE9, 0x39, 0xD8, 0x25, 0x61, 0x9C, 0x10, 0xBA, 0x5A, 0xAB, 0x86, 0x4C, 0xF7, 
          0xD0, 0x73, 0xCD, 0xEF, 0x59, 0x9A, 0xE8, 0xE7, 0xBC, 0xEA, 0xF1, 0xAF, 0xA0, 0x90, 
          0x9D, 0x57, 0xF1, 0x0C, 0x3E, 0x82, 0xC2, 0x2C, 0xF0, 0x1A, 0xDA, 0x6D, 0x40, 0xD4, 
          0x29, 0xBE, 0x42, 0x4B, 0xA6, 0x09, 0x31, 0x28, 0xA1, 0xBD, 0x58, 0x00, 0x69, 0x89, 
          0xDA, 0xD6, 0x80, 0x72, 0x93, 0xE1, 0x7D, 0x2E, 0xB7, 0xFD, 0xC3, 0x40, 0x0A, 0xAE, 
          0x52, 0x44, 0xC1, 0x3D, 0x7F, 0x6A, 0x77, 0x59, 0x72, 0xA4, 0xD1, 0x77, 0x93, 0x17, 
          0x1F, 0xAB, 0x99, 0xB1, 0x26, 0x81, 0xD5, 0x02, 0x03, 0x01, 0x00, 0x01 }));
      _station = _station ?? new Station(this, key, _masterPassword);

      try {
        return _station.ImportData(ImportVoterData(voterDataPath), ImportPrecinctData(precinctDataPath));
      } catch (Exception e) {
        Console.WriteLine("Data import exception: " + e);
        return false;
      }
    }

    /// <summary>
    /// Import the election data from a CSV file (note that this is
    /// specific to the demo version and will not work in general).
    /// </summary>
    /// <param name="filePath">
    /// the file path of the voter data
    /// </param>
    /// <returns>
    /// the voter data as a IEnumerable of EncryptedVoterData
    /// </returns>
    public IEnumerable<Voter> ImportVoterData(string filePath) { 
      List<Voter> result = new List<Voter>();
      using (StreamReader sr = new StreamReader(filePath))
      {
        string line;
        char[] delimiters = new char[] { ',' };
        int count = 0;
        line = sr.ReadLine(); // throw out the first line
        try
        {
          while ((line = sr.ReadLine()) != null)
          {
            count = count + 1;
            if (count % 1000 == 0) {
              Console.WriteLine("Read {0} voters from input data", count);
            }
            string[] parts = line.Split(delimiters);
            Voter v = new Voter();
            v.VoterId = Int32.Parse(parts[0]);
            v.LastName = parts[1];
            v.FirstName = parts[2];
            v.MiddleName = parts[3];
            v.Suffix = parts[4];
            v.DateOfBirth = DateTime.Parse(parts[5]);
            v.ProtectedAddress = Boolean.Parse(parts[6]);
            v.Address = parts[7];
            v.Municipality = parts[8];
            v.ZipCode = parts[9];
            v.Status = parts[10];
            DateTime dt;
            DateTime.TryParse(parts[11], out dt);
            v.EligibleDate = dt;
            v.MustShowId = Boolean.Parse(parts[12]); 
            v.DriversLicense = parts[13];
            v.StateId = Int32.Parse(parts[14]);
            v.PrecinctSub = parts[15];
            v.BallotStyle = parts[16];
            v.Voted = Boolean.Parse(parts[17]);
            v.Absentee = Boolean.Parse(parts[18]);
            v.ReturnStatus = parts[19];
            v.PollbookStatus = 0;
            result.Add(v);
          }
        }
        catch (Exception e)
        {
          Console.Write(e);
          throw e;
        }
      }

      return result;
    }

    public IEnumerable<Precinct> ImportPrecinctData(string filePath) {
      List<Precinct> result = new List<Precinct>();
      using (StreamReader sr = new StreamReader(filePath)) {
        string line;
        char[] delimiters = new char[] { ',' };
        int count = 0;
        line = sr.ReadLine(); // throw out the first line
        try {
          while ((line = sr.ReadLine()) != null) {
            count = count + 1;
            if (count % 50 == 0) {
              Console.WriteLine("Read {0} precincts from input data", count);
            }
            string[] parts = line.Split(delimiters);
            Precinct p = new Precinct();
            p.PrecinctSplitId = parts[0];
            p.LocationName = parts[1];
            p.Address = parts[2];
            p.CityStateZIP = parts[3] + ", " + parts[4] + "  " + parts[5];

            result.Add(p);
          }
        } catch (Exception e) {
          Console.Write(e);
          throw e;
        }
      }

      return result;
    }

    /// <summary>
    /// An identifying string for this DVL.
    /// </summary>
    /// <returns>
    /// An identifying string for this DVL.
    /// </returns>
    public string IdentifyingString() {
      string result = "UNKNOWN";
      if (_station != null) {
        result = _station.Communicator.GetIdentifyingString();
      }
      return result;
    }

    /// <summary>
    /// An identifying string for another DVL.
    /// </summary>
    /// <param name="station">
    /// The IP end point of the other DVL.
    /// </param>
    /// <returns>
    /// An identifying string for another DVL.
    /// </returns>    
    public string IdentifyingStringForStation(IPEndPoint station) {
      string result = "UNKNOWN";
      if (_station != null) {
        result = _station.Communicator.GetIdentifyingStringForStation(station);
      }
      return result;
    }

    /// <summary>
    /// Checks if a entered password matches the master password
    /// </summary>
    /// <param name="typedPassword">
    /// the entered password
    /// </param>
    /// <returns>
    /// whether or not it matches the master password
    /// </returns>
    public bool IsMasterPWCorrect(string typedPassword) { return _station != null && _station.ValidMasterPassword(typedPassword); }

    /// <summary>
    /// The is now manager.
    /// </summary>
    public void IsNowManager() {
      if (BallotRequestPage != null) {
        BallotRequestPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate {
            FlexibleMessageBox.Show(_stationNativeWindow, "This station will now become the manager.");
            BallotRequestPage.BecomeManager(); 
          }));
      }
    }

    /// <summary>
    /// Checks whether or not a machine is reachable
    /// </summary>
    /// <param name="ip">
    /// the IP address of the machine
    /// </param>
    /// <returns>
    /// whether or not the machine is active
    /// </returns>
    public bool IsStationActive(IPEndPoint ip) { return _station.StationActive(ip); }

    /// <summary>
    /// When a manager wants to promot another station to be the new manager this is called
    /// </summary>
    /// <param name="ip">
    /// The IP address of the new manager
    /// </param>
    /// <returns>
    /// whether or not the promotion was succesful
    /// </returns>
    public bool MakeManager(IPEndPoint ip) {
      if (_station.StationActive(ip)) {
        _station.PromoteNewManager(ip);
        return true;
      }

      return false;
    }

    public void ConvertToStation() {
      if (ManagerOverviewPage != null) {
        ManagerOverviewPage.ConvertToStation();
      }
    }

    /// <summary>
    /// When a manager want to start the election this method should be called
    /// to announce the start to all stations.
    /// </summary>
    public void ManagerAnnounceStartElection() { _station.AnnounceStartElection(); }

    /// <summary>
    /// The manager exchanging key.
    /// </summary>
    /// <param name="ip">
    /// The ip.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string ManagerExchangingKey(IPEndPoint ip) { return WaitingForManagerPage != null ? WaitingForManagerPage.IncomingConnection(ip) : string.Empty; }

    /// <summary>
    /// Marks a station as connnected in the list
    /// </summary>
    /// <param name="ip">
    /// the p address of the station
    /// </param>
    public void MarkAsConnected(IPEndPoint ip) {
      _station.SetPeerStatus(ip, "Connected");
      if (OverviewPage != null) {
        OverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { OverviewPage.RefreshGrid(); }));
      }

      if (ManagerOverviewPage != null) {
        ManagerOverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { ManagerOverviewPage.RefreshGrid(); }));
      }
    }

    /// <summary>
    /// The not enough peers.
    /// </summary>
    public void NotEnoughPeers() {
      if (BallotRequestPage != null) {
        BallotRequestPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal,
          new Action(delegate { BallotRequestPage.Blocked = true; BallotRequestPage.WaitingLabel.Content = "Not Enough Stations"; }));
        BallotRequestPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { BallotRequestPage.checkValidityButton.IsEnabled = false; }));
      }

      if (ManagerOverviewPage != null) {
        ManagerOverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { ManagerOverviewPage.Blocked = true; }));
      }
    }

    /// <summary>
    /// Called when the manager wants to remove a station from the network
    /// </summary>
    /// <param name="ip">
    /// the IP adress of the station to be removed
    /// </param>
    public void RemoveStation(IPEndPoint ip) {_station.RemovePeer(ip, true); }

    /// <summary>
    /// This method is called when a voter wants to request a ballot after entering their voternumber and CPR number
    /// </summary>
    /// <param name="voterNumber">
    /// the voternumber of the voter
    /// </param>
    public void RequestStatusChange(Voter voter, VoterStatus voterStatus) {
      _station.RequestStatusChange(voter, voterStatus);
    }

    /// <summary>
    /// The show password on manager.
    /// </summary>
    /// <param name="password">
    /// The password.
    /// </param>
    public void ShowPasswordOnManager(string password, IPEndPoint station) {
      string name = IdentifyingStringForStation(station);
      if (OverviewPage != null) {
        OverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(
            delegate { OverviewPage.SetPasswordLabel("Enter this password at Station " + name +": " + password); }));
      }

      if (ManagerOverviewPage != null) {
        ManagerOverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(
            delegate { ManagerOverviewPage.SetPasswordLabel("Enter this password at Station " + name + ":\n" + password); }));
      }
    }

    /// <summary>
    /// The show password on station.
    /// </summary>
    /// <param name="password">
    /// The password.
    /// </param>
    public void ShowPasswordOnStation(string password, IPEndPoint manager) {
      if (WaitingForManagerPage != null) {
        WaitingForManagerPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(
            delegate { WaitingForManagerPage.SetPasswordLabel("Enter this password at Manager " + IdentifyingStringForStation(manager) + ": " + password); }));
      }
    }

    /// <summary>
    /// The shutdown.
    /// </summary>
    public void Shutdown() {
      FlexibleMessageBox.Show(_stationNativeWindow,
        "Something has gone wrong with the system, shutting down", 
        "Shutting Down", 
        MessageBoxButtons.OK, 
        MessageBoxIcon.Error);
      Environment.Exit(0);
    }

    /// <summary>
    /// The station exchanging key.
    /// </summary>
    /// <param name="ip">
    /// The ip.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string StationExchangingKey(IPEndPoint ip) {
      if (OverviewPage != null) {
        OverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { OverviewPage.SetPasswordLabel(string.Empty); }));
        return OverviewPage.IncomingReply(ip);
      }

      if (ManagerOverviewPage != null) {
        ManagerOverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { ManagerOverviewPage.SetPasswordLabel(string.Empty); }));
        return ManagerOverviewPage.IncomingReply(ip);
      }

      return string.Empty;
    }

    /// <summary>
    /// The station removed.
    /// </summary>
    public void StationRemoved() {
      FlexibleMessageBox.Show(_stationNativeWindow,
        "This station has been shut down by the manager", 
        "Station Shut Down", 
        MessageBoxButtons.OK, 
        MessageBoxIcon.Warning);
      if (WaitingForManagerPage != null) {
        WaitingForManagerPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { WaitingForManagerPage.StationRemoved(); }));
      }

      if (BallotRequestPage != null) {
        BallotRequestPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { BallotRequestPage.StationRemoved(); }));
      }
    }

    public void Synchronizing(IPEndPoint ip) {
      _station.SetPeerStatus(ip, "Synchronizing Election Data");
      if (OverviewPage != null) {
        OverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal,
          new Action(delegate { OverviewPage.RefreshGrid(); OverviewPage.UpdateControls(); }));
      }
      if (ManagerOverviewPage != null) {
        ManagerOverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal,
          new Action(delegate { ManagerOverviewPage.RefreshGrid(); ManagerOverviewPage.UpdateControls(); }));
      }
    }

    public void DoneSynchronizing(IPEndPoint ip) {
      _station.SetPeerStatus(ip, "Connected");
      if (OverviewPage != null) {
        OverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal,
          new Action(delegate { OverviewPage.RefreshGrid(); OverviewPage.UpdateControls(); }));
      }
      if (ManagerOverviewPage != null) {
        ManagerOverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal,
          new Action(delegate { ManagerOverviewPage.RefreshGrid(); ManagerOverviewPage.UpdateControls();  }));
        if (_station.ElectionInProgress) {
          _station.Communicator.Send(new StartElectionCommand(_station.Address), ip);
        }

      }
    }

    public void SyncComplete() {
      if (WaitingForManagerPage != null) {
        WaitingForManagerPage.CenterLabel.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal,
            new Action(delegate { WaitingForManagerPage.CenterLabel.Content = "Waiting for election to start..."; 
              WaitingForManagerPage.PasswordLabel.Content = String.Empty;  }));
      }
    }

    public void ResetBallotRequestPage() {
      if (BallotRequestPage != null) {
        BallotRequestPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal,
          new Action(delegate { BallotRequestPage.RecoverFromManagerChange(); }));
      }
    }

    public void RefreshStatistics() {
      if (ManagerOverviewPage != null) {
        ManagerOverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal,
          new Action(delegate { ManagerOverviewPage.RefreshStatistics(); }));
      }
    }

    public void RefreshPeers() {
      if (ManagerOverviewPage != null) {
        ManagerOverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal,
          new Action(delegate { ManagerOverviewPage.ManagerstationGrid.Items.Refresh(); ManagerOverviewPage.UpdateControls(); }));
      }
      if (OverviewPage != null) {
        OverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal,
          new Action(delegate { OverviewPage.stationGrid.Items.Refresh(); OverviewPage.UpdateControls();  }));
      }
    }

    #endregion
  }
}

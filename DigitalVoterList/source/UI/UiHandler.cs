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
  using System.Windows;

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

    public readonly string IPAddressString;

    /// <summary>
    /// The _station window.
    /// </summary>
    public readonly StationWindow _stationWindow;

    /// <summary>
    /// The _master password.
    /// </summary>
    private string _masterPassword;

    /// <summary>
    /// The _station.
    /// </summary>
    public Station _station;

    private bool _hasData = false;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="UiHandler"/> class.
    /// </summary>
    /// <param name="stationWindow">
    /// The station window.
    /// </param>
    public UiHandler(StationWindow stationWindow) { 
      this._stationWindow = stationWindow;
      IPAddressString = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(ip =>
        ip.AddressFamily == AddressFamily.InterNetwork).ToString();
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// When a manager want to end the election this method should be called
    /// to announce the end to all stations.
    /// </summary>
    public void AnnounceEndElection() { this._station.AnnounceEndElection(); }

    public void BallotRequestReply(VoterNumber vn, bool success, VoterStatus oldStatus, VoterStatus newStatus) {
      this.BallotRequestReply(this._station.Database.GetVoterByVoterNumber(vn), success, oldStatus, newStatus);
    }

    /// <summary>
    /// The ballot request reply.
    /// </summary>
    /// <param name="successful">
    /// The hand out ballot.
    /// </param>
    public void BallotRequestReply(Voter voter, bool success, VoterStatus oldStatus, VoterStatus newStatus) {
      if (this.BallotRequestPage != null) {
        this.BallotRequestPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.BallotRequestPage.BallotResponse(voter, success, oldStatus, newStatus); }));
      }
    }

    /// <summary>
    /// Creates a new station and makes it start listening
    /// </summary>
    public void CreateNewStation() { this._station = new Station(this); }

    /// <summary>
    /// Gets he IP adresses of the machines in the local network running this application
    /// </summary>
    /// <returns>a list of IP adresses</returns>
    public void DiscoverPeers() { _station.DiscoverPeers(); }

    /// <summary>
    /// disposes the current station
    /// </summary>
    public void DisposeStation() {
      if (this._station != null) this._station.Dispose();
      this._station = null;
    }

    /// <summary>
    /// The election ended.
    /// </summary>
    public void ElectionEnded() {
      if (this.BallotRequestPage != null) {
        this.BallotRequestPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.BallotRequestPage.EndElection(); }));
      }
    }

    /// <summary>
    /// The election started.
    /// </summary>
    public void ElectionStarted() {
      if (this.WaitingForManagerPage != null) {
        this.WaitingForManagerPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.WaitingForManagerPage.StartElection(); }));
      }
    }

    /// <summary>
    /// The enough peers.
    /// </summary>
    public void EnoughPeers() {
      if (this.BallotRequestPage != null) {
        this.BallotRequestPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.BallotRequestPage.Blocked = false;
          BallotRequestPage.WaitingLabel.Content = ""; 
        }));
      }

      if (this.ManagerOverviewPage != null) {
        this.ManagerOverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.ManagerOverviewPage.Blocked = false; }));
      }
    }

    /// <summary>
    /// Checks if there are enough stations in the peerlist
    /// </summary>
    /// <returns>wheter there are enough stations is hte peerlist</returns>
    public bool EnoughStations() { return this._station.EnoughStations; }

    /// <summary>
    /// When a manager wants to connect to a station, this is called to exchange the public keys
    /// </summary>
    /// <param name="ip">
    /// The IP address of the station
    /// </param>
    public void ExchangeKeys(IPEndPoint ip) { this._station.ExchangePublicKeys(ip); }

    /// <summary>
    /// Export data in the format IEnumerable of EncryptedVoterData to a file
    /// </summary>
    /// <param name="data">
    /// the voter data to be exported
    /// </param>
    /// <param name="filePath">
    /// the file destination
    /// </param>
    public void ExportData(IEnumerable<Voter> data, string filePath) { Bytes.From(data).ToFile(filePath); }

    /// <summary>
    /// When a manager wants to export voter data this is called
    /// </summary>
    /// <param name="filePath">
    /// the destination filepath
    /// </param>
    public void ExportData(string filePath) {
      if (this._station != null) this.ExportData(this._station.Database.AllVoters, filePath);
      else MessageBox.Show("You can not export data at this time.", "Operation Not Allowed", MessageBoxButton.OK);
    }

    /// <summary>
    /// This methods asks the station for a password it can use as master password
    /// </summary>
    /// <returns>the master password</returns>
    public string GeneratePassword() {
      this._masterPassword = Crypto.GeneratePassword();
      return this._masterPassword;
    }

    /// <summary>
    /// Asks the _station for the list of peers as IPEndpoints
    /// </summary>
    /// <returns>the list of peers as IPEndpoints</returns>
    public IEnumerable<IPEndPoint> GetPeerlist() { return this._station.Peers.Keys; }

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
      this._station = this._station ?? new Station(this, key, this._masterPassword);
      this._masterPassword = null;

      try {
        this._station.Database.Import(this.ImportVoterData(voterDataPath));
        this._station.Database.Import(this.ImportPrecinctData(precinctDataPath));
        _hasData = true;
        return true;
      } catch (Exception) {
        return false;
      }
    }

    /// <summary>
    /// Import the election data from a |-delimited "CSV" file (note that this is
    /// specific to the Dallas demo and will not work in general).
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
        char[] delimiters = new char[] { '|' };
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
            // part 0 is "ID", skip it
            v.VoterId = Int32.Parse(parts[1]);
            // part 2 is "address_id"
            v.Status = parts[3];
            v.LastName = parts[4];
            v.FirstName = parts[5];
            v.MiddleName = parts[6];
            v.Suffix = parts[7];
            v.DateOfBirth = DateTime.Parse(parts[8]);
            // part 9 is "dateofreg"
            DateTime dt;
            DateTime.TryParse(parts[10], out dt);
            v.EligibleDate = dt;
            // part 11 is "unit"
            // part 12 is "unitno"
            v.MustShowId = Boolean.Parse(parts[13]); // actually "registeredbymail"
            v.Absentee = Boolean.Parse(parts[14]);
            v.ProtectedAddress = Boolean.Parse(parts[15]);
            // part 16 is "party"
            // part 17 is "ssn"
            // part 18 is "ssnrev"
            v.DriversLicense = parts[19];
            // part 20 is "dlrev"
            Int32 house;
            StringBuilder address = new StringBuilder();
            if (Int32.TryParse(parts[21], out house)) {
              address.Append(house.ToString());
            }
            // part 22 is "signature"
            // part 23 is "signaturefilename"
            v.Voted = Boolean.Parse(parts[24]);
            // part 25 is "maildate"
            // part 26 is "returndate"
            v.ReturnStatus = parts[27];
            Int32 bs;
            if (Int32.TryParse(parts[28], out bs)) {
              v.BallotStyle = bs;
            } else {
              v.BallotStyle = -1;
            }
            v.PrecinctSub = parts[29];
            // part 30 is precinct
            // part 31 is precsub
            string bldg = parts[32];
            string predir = parts[33];
            string streetname = parts[34];
            string streettype = parts[35];
            string postdir = parts[36];
            // now we can make the address
            if (bldg.Length > 0) {
              if (address.Length > 0) { address.Append(" "); }
              address.Append(bldg);
            }
            if (predir.Length > 0) {
              if (address.Length > 0) { address.Append(" "); }
              address.Append(predir);
            }
            if (streetname.Length > 0) {
              if (address.Length > 0) { address.Append(" "); }
              address.Append(streetname);
            }
            if (streettype.Length > 0) {
              if (address.Length > 0) { address.Append(" "); }
              address.Append(streettype);
            }
            if (postdir.Length > 0) {
              if (address.Length > 0) { address.Append(" "); }
              address.Append(postdir);
            }
            v.Address = address.ToString();
            v.Municipality = parts[37];
            v.ZipCode = parts[38];
            // part 39 is parcel_address
            // part 40 is poll_code
            // part 41 is election_code-sub
            v.StateId = Int32.Parse(parts[42]);
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
        char[] delimiters = new char[] { '|' };
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
            // part 0 is "Index", skip it
            // part 1 is "ElectionID", skip it
            p.PrecinctSplitId = parts[2];
            // part 3 is "Precinct", skip it
            // part 4 is "SubGroup", skip it
            // part 5 is "MPCT", skip it
            p.LocationName = parts[6];
            p.Address = parts[7];
            p.CityStateZIP = parts[8] + ", " + parts[9] + "  " + parts[10];
            // everything else gets skipped

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
    /// Checks if a entered password matches the master password
    /// </summary>
    /// <param name="typedPassword">
    /// the entered password
    /// </param>
    /// <returns>
    /// whether or not it matches the master password
    /// </returns>
    public bool IsMasterPWCorrect(string typedPassword) { return this._station != null && this._station.ValidMasterPassword(typedPassword); }

    /// <summary>
    /// The is now manager.
    /// </summary>
    public void IsNowManager() {
      if (this.BallotRequestPage != null) {
        this.BallotRequestPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate {
            MessageBox.Show("Unable to contact the manager. This station will now become the manager.");
            this.BallotRequestPage.BecomeManager(); 
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
    public bool IsStationActive(IPEndPoint ip) { return this._station.StationActive(ip); }

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
      if (this._station.StationActive(ip)) {
        this._station.PromoteNewManager(ip);
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
    public void ManagerAnnounceStartElection() { this._station.AnnounceStartElection(); }

    /// <summary>
    /// The manager exchanging key.
    /// </summary>
    /// <param name="ip">
    /// The ip.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string ManagerExchangingKey(IPEndPoint ip) { return this.WaitingForManagerPage != null ? this.WaitingForManagerPage.IncomingConnection(ip) : string.Empty; }

    /// <summary>
    /// Marks a station as connnected in the list
    /// </summary>
    /// <param name="ip">
    /// the p address of the station
    /// </param>
    public void MarkAsConnected(IPEndPoint ip) {
      _station.SetPeerStatus(ip, "Connected");
      if (this.OverviewPage != null) {
        this.OverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.OverviewPage.RefreshGrid(); }));
      }

      if (this.ManagerOverviewPage != null) {
        this.ManagerOverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.ManagerOverviewPage.RefreshGrid(); }));
      }
    }

    /// <summary>
    /// The not enough peers.
    /// </summary>
    public void NotEnoughPeers() {
      if (this.BallotRequestPage != null) {
        this.BallotRequestPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal,
          new Action(delegate { this.BallotRequestPage.Blocked = true; BallotRequestPage.WaitingLabel.Content = "Not Enough Stations"; }));
        this.BallotRequestPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.BallotRequestPage.checkValidityButton.IsEnabled = false; }));
      }

      if (this.ManagerOverviewPage != null) {
        this.ManagerOverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.ManagerOverviewPage.Blocked = true; }));
      }
    }

    /// <summary>
    /// Called when the manager wants to remove a station from the network
    /// </summary>
    /// <param name="ip">
    /// the IP adress of the station to be removed
    /// </param>
    public void RemoveStation(string ip) { this._station.AnnounceRemovePeer(new IPEndPoint(IPAddress.Parse(ip), 62000)); }

    /// <summary>
    /// This method is called when a voter wants to request a ballot after entering their voternumber and CPR number
    /// </summary>
    /// <param name="voterNumber">
    /// the voternumber of the voter
    /// </param>
    public void RequestStatusChange(Voter voter, VoterStatus voterStatus) {
      this._station.RequestStatusChange(voter, voterStatus);
    }

    /// <summary>
    /// The show password on manager.
    /// </summary>
    /// <param name="password">
    /// The password.
    /// </param>
    public void ShowPasswordOnManager(string password) {
      if (this.OverviewPage != null) {
        this.OverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(
            delegate { this.OverviewPage.SetPasswordLabel("Enter this password at the station: " + password); }));
      }

      if (this.ManagerOverviewPage != null) {
        this.ManagerOverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(
            delegate { this.ManagerOverviewPage.SetPasswordLabel("Enter this password at the station: " + password); }));
      }
    }

    /// <summary>
    /// The show password on station.
    /// </summary>
    /// <param name="password">
    /// The password.
    /// </param>
    public void ShowPasswordOnStation(string password) {
      if (this.WaitingForManagerPage != null) {
        this.WaitingForManagerPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(
            delegate { this.WaitingForManagerPage.SetPasswordLabel("Enter this password at the manager: " + password); }));
      }
    }

    /// <summary>
    /// The shutdown.
    /// </summary>
    public void Shutdown() {
      MessageBox.Show(
        "Something has gone wrong with the system, shutting down", 
        "Shutting Down", 
        MessageBoxButton.OK, 
        MessageBoxImage.Error);
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
      if (this.OverviewPage != null) {
        this.OverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.OverviewPage.SetPasswordLabel(string.Empty); }));
        return this.OverviewPage.IncomingReply(ip);
      }

      if (this.ManagerOverviewPage != null) {
        this.ManagerOverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.ManagerOverviewPage.SetPasswordLabel(string.Empty); }));
        return this.ManagerOverviewPage.IncomingReply(ip);
      }

      return string.Empty;
    }

    /// <summary>
    /// The station removed.
    /// </summary>
    public void StationRemoved() {
      MessageBox.Show(
        "This station has been shut down by the manager", 
        "Station Shut Down", 
        MessageBoxButton.OK, 
        MessageBoxImage.Warning);
      if (this.WaitingForManagerPage != null) {
        this.WaitingForManagerPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.WaitingForManagerPage.StationRemoved(); }));
      }

      if (this.BallotRequestPage != null) {
        this.BallotRequestPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.BallotRequestPage.StationRemoved(); }));
      }
    }

    public void Synchronizing(IPEndPoint ip) {
      _station.SetPeerStatus(ip, "Synchronizing Election Data");
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

    public void DoneSynchronizing(IPEndPoint ip) {
      _station.SetPeerStatus(ip, "Ready");
      if (OverviewPage != null) {
        OverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal,
          new Action(delegate { OverviewPage.RefreshGrid(); }));
      }
      if (ManagerOverviewPage != null) {
        ManagerOverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal,
          new Action(delegate { ManagerOverviewPage.RefreshGrid(); }));
        if (_station.ElectionInProgress) {
          _station.Communicator.Send(new StartElectionCommand(_station.Address), ip);
        }

      }
    }

    public void SyncComplete() {
      _hasData = true;
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

    #endregion
  }
}

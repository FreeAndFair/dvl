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
  using System.Windows;

  using Aegis_DVL;
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
    /// The ballot cpr request window.
    /// </summary>
    public BallotCPRRequestWindow BallotCPRRequestWindow;

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

    /// <summary>
    /// The waiting for manager page.
    /// </summary>
    public WaitingForManagerPage WaitingForManagerPage;

    public readonly string IPAddressString;

    /// <summary>
    /// The _station window.
    /// </summary>
    private readonly StationWindow _stationWindow;

    /// <summary>
    /// The _master password.
    /// </summary>
    private string _masterPassword;

    /// <summary>
    /// The _station.
    /// </summary>
    private Station _station;

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

    public void BallotRequestReply(VoterNumber vn, bool handOutBallot) {
      this.BallotRequestReply(this._station.Database.GetVoterByVoterNumber(vn), handOutBallot);
    }

    /// <summary>
    /// The ballot request reply.
    /// </summary>
    /// <param name="handOutBallot">
    /// The hand out ballot.
    /// </param>
    public void BallotRequestReply(Voter voter, bool handOutBallot) {
      if (this.BallotRequestPage != null) {
        this.BallotRequestPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.BallotRequestPage.BallotResponse(voter, handOutBallot); }));
      }

      if (this.ManagerOverviewPage != null) {
        this.ManagerOverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.ManagerOverviewPage.BallotResponse(voter, handOutBallot); }));
      }

      if (this.BallotCPRRequestWindow != null) {
        this.BallotCPRRequestWindow.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.BallotCPRRequestWindow.BallotResponse(voter, handOutBallot); }));
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
    public IEnumerable<IPEndPoint> DiscoverPeers() { return this._station.DiscoverPeers(); }

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
      this._stationWindow.Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, 
        new Action(delegate { this._stationWindow.MarkVoterMenuItem.IsEnabled = true; }));

      if (this.BallotRequestPage != null) {
        this.BallotRequestPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.BallotRequestPage.Blocked = false; }));
        this.BallotRequestPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.BallotRequestPage.WaitingLabel.Content = string.Empty; }));
      }

      if (this.ManagerOverviewPage != null) {
        this.ManagerOverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.ManagerOverviewPage.Blocked = false; }));
        this.ManagerOverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.ManagerOverviewPage.WaitingLabel.Content = string.Empty; }));
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
      if (this._station != null) this.ExportData(this._station.Database.AllData, filePath);
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
    /// <param name="dataPath">
    /// the file path of the data to be imported
    /// </param>
    /// <param name="keyPath">
    /// the file path of the encryption key
    /// </param>
    /// <returns>
    /// whether or not the import was succesful
    /// </returns>
    public bool ImportData(string dataPath, string keyPath) {
      this._station = this._station ?? new Station(this, keyPath, this._masterPassword);
      this._masterPassword = null;

      try {
        this._station.Database.Import(this.ImportElectionData(dataPath));
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
    public IEnumerable<Voter> ImportElectionData(string filePath) { 
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
            // part 15 is "protectedaddress"
            // part 16 is "party"
            // part 17 is "ssn"
            // part 18 is "ssnrev"
            v.DriversLicense = parts[19];
            // part 20 is "dlrev"
            // part 21 is "house"
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
            // part 32 is streetbldg
            // part 33 is streetpredir
            // part 34 is streetname
            // part 35 is streettype
            // part 36 is streetpostdir
            // part 37 is municipality
            // part 38 is zip
            // part 39 is parcel_address
            // part 40 is poll_code
            // part 41 is election_code-sub
            v.StateId = Int32.Parse(parts[42]);
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
          new Action(delegate { this.BallotRequestPage.BecomeManager(); }));
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
      if (this.OverviewPage != null) {
        this.OverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.OverviewPage.MarkAsConnected(ip); }));
      }

      if (this.ManagerOverviewPage != null) {
        this.ManagerOverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.ManagerOverviewPage.MarkAsConnected(ip); }));
      }
    }

    /// <summary>
    /// The not enough peers.
    /// </summary>
    public void NotEnoughPeers() {
      this._stationWindow.Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, 
        new Action(delegate { this._stationWindow.MarkVoterMenuItem.IsEnabled = false; }));

      if (this.BallotRequestPage != null) {
        this.BallotRequestPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.BallotRequestPage.Blocked = true; }));
        this.BallotRequestPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.BallotRequestPage.checkValidityButton.IsEnabled = false; }));
        this.BallotRequestPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.BallotRequestPage.WaitingLabel.Content = "There are not enough stations connected"; }));
      }

      if (this.ManagerOverviewPage != null) {
        this.ManagerOverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.ManagerOverviewPage.Blocked = true; }));
        this.ManagerOverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(delegate { this.ManagerOverviewPage.checkValidityButton.IsEnabled = false; }));
        this.ManagerOverviewPage.Dispatcher.Invoke(
          System.Windows.Threading.DispatcherPriority.Normal, 
          new Action(
            delegate { this.ManagerOverviewPage.WaitingLabel.Content = "There are not enough stations connected"; }));
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
    public void RequestBallot(string voterNumber) {
      var vn = new VoterNumber(Int32.Parse(voterNumber));

      if (this._station.Database[vn] == BallotStatus.NotReceived) this._station.RequestBallot(vn);
      else this.BallotRequestReply(this._station.Database.GetVoterByVoterNumber(vn), false);
    }

    /*
    /// <summary>
    /// This method is called when an election offical wants to mark a voter by only using their CPR number.
    /// The election official will also need to enter the master password.
    /// </summary>
    /// <param name="cpr">
    /// the CPR number of the voter
    /// </param>
    /// <param name="masterPassword">
    /// the systems master password
    /// </param>
    // note that the master password is not used at all for this at the moment...
    public void RequestBallotOnlyCPR(string cpr, string masterPassword) {
      var ncpr = new VoterNumber(Int32.Parse(cpr));

      if (this._station.Database[ncpr] == BallotStatus.NotReceived) this._station.RequestBallot(ncpr);
      else this.BallotRequestReply(false);
    }
    */

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

    #endregion
  }
}

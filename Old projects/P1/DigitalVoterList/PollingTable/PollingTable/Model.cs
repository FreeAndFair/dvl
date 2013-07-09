
// -----------------------------------------------------------------------
// <copyright file="Model.cs" company="">
// Author: Claes Martinsen.
// </copyright>
// -----------------------------------------------------------------------

namespace PollingTable.PollingTable
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;

    using DBComm.DBComm;
    using DBComm.DBComm.DAO;
    using DBComm.DBComm.DO;

    /// <summary>
    /// The model class is responsible for the logic in polling table. 
    /// The model class communicates with the DAO to update when voters has been registered or unregistered.
    /// It also provide utility methods that test the model's current state.
    /// </summary>
    public class Model
    {
        private const string Path = "c:/SetupDVL.conf";  //The path where the setup.conf file will be stored.

        public VoterDO currentVoter; //The current voter to be handled.

        private static PessimisticVoterDAO staticPvdao; //The DAO used to update the voter's status.

        private string adminPass = ""; //The password used to access the voter box.

        //contains setup information for the polling table. Voter Box address and table number
        private SetupInfo setupInfo = new SetupInfo("localhost", 0);

        public delegate void VoterChangedHandler(VoterDO voter);
        public delegate void SetupInfoChangedHandler(SetupInfo setupInfo);
        public delegate void ConnectionErrorHandler();

        public event VoterChangedHandler CurrentVoterChanged; //Notify when the current voter changes.
        public event SetupInfoChangedHandler SetupInfoChanged; //Notify when the setup information is changed. 
        public event ConnectionErrorHandler ConnectionError; //Notify when a connection error occours. 

        #region Utility methods to validate user input

        /// <summary>
        /// Validate if the length of the cprno is correct.
        /// </summary>
        /// <param name="cpr">the cprno to be validated</param>
        /// <returns>false if cpr is not valid, true if it is.</returns>
        public static bool CprLengtVal(uint cpr)
        {
            bool val = (cpr.ToString().Length > 12 || cpr.ToString().Length < 10);
            return val;
        }

        /// <summary>
        /// Validate if the cprno does not contain letters.
        /// </summary>
        /// <param name="cpr">The cprno the be validated</param>
        /// <returns>false if cpr is not valid, true if it is.</returns>
        public static bool CprLetterVal(string cpr)
        {
            UInt64 result;
            bool res = UInt64.TryParse(cpr, out result);
            if (!res) return false;
            return true;
        }

        #endregion

        /// <summary>
        /// Closes the pessimistic voter DAO's transaction, 
        /// if it is already open.
        /// </summary>
        public static void cleanUpDAO()
        {
            if (staticPvdao.TransactionStarted()) staticPvdao.EndTransaction();
        }

        /// <summary>
        /// Initializes and starts the pessimistic voter DAO's transaction.
        /// </summary>
        public void initializeStaticDAO()
        {
            staticPvdao = new PessimisticVoterDAO(setupInfo.Ip, adminPass); //Initialize the pvdao.
            staticPvdao.StartTransaction();
        }

        /// <summary>
        /// Updates the current voter with the voter matching the cprno.
        ///  
        /// </summary>
        /// <param name="cprno">The cpr number of the voter to be found</param>
        public void FindVoter(uint cprno)
        {
            Contract.Requires(cprno != null);
            Contract.Ensures(cprno == currentVoter.PrimaryKey);
            try
            {
                currentVoter = staticPvdao.Read(cprno);

                //Update the current voter with the found voter
                CurrentVoterChanged(currentVoter);
                //Update log with read entry
                this.UpdateLog(ActivityEnum.R);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ConnectionError(); // notify that the connection has been lost.
                return;
            }
        }

        /// <summary>
        /// Fetches the voter matching the cprno.
        /// </summary>
        /// <param name="cprno">The cprno of the voter to be fetched in the voter box.</param>
        /// <returns>The voter matching </returns>
        public VoterDO FetchVoter(uint cprno)
        {
            Contract.Requires(cprno != null);
            VoterDO voter;

            try
            {
                voter = staticPvdao.Read(cprno);
            }
            catch (Exception)
            {
                voter = new VoterDO();
                ConnectionError();
            }
            return voter;

        }

        /// <summary>
        /// Registeres the current voter as registered in the voter box.
        /// </summary>
        public void RegisterCurrentVoter()
        {
            Contract.Requires(currentVoter.Voted == false);
            Contract.Requires(currentVoter != null);
            Contract.Ensures(currentVoter.Voted == true);
            try
            {
                staticPvdao.Update((uint)currentVoter.PrimaryKey, true);

                //refresh the current voter to reflect the new voted status
                currentVoter = FetchVoter((uint)currentVoter.PrimaryKey);

                //Update log with write entry
                staticPvdao.EndTransaction();
                this.UpdateLog(ActivityEnum.W); //Update log with nregister entry
            }
            catch (Exception)
            {
                ConnectionError();
                return;
            }
        }

        /// <summary>
        /// Unregisters the current voter.
        /// </summary>
        public void UnregisterCurrentVoter()
        {
            Contract.Requires(currentVoter.Voted == true);
            Contract.Requires(currentVoter != null);
            Contract.Ensures(currentVoter.Voted == false);

            try
            {
                staticPvdao.Update((uint)currentVoter.PrimaryKey, false);
                //refresh the current voter to reflect the new voted status
                currentVoter = FetchVoter((uint)currentVoter.PrimaryKey);
                staticPvdao.EndTransaction();
                this.UpdateLog(ActivityEnum.U); //Update log with unregister entry
            }
            catch (Exception)
            {
                ConnectionError();
                return;
            }
        }

        /// <summary>
        /// Reads setup information from the setup.conf file.
        /// </summary>
        public void ReadConfig()
        {
            //If it doesn't exist, create a new one (with blank lines).
            if (!File.Exists(Path))
            {
                string[] write = { };
                File.WriteAllLines(Path, write);
            }
            //try to read the file. 
            string[] arr = File.ReadAllLines(Path);
            if (arr.Length > 0)
            {
                setupInfo.Ip = arr[0];

                //test if the read string can be parsed to an int
                string str = arr[1];
                uint i;
                bool res = UInt32.TryParse(str, out i);
                if (res) setupInfo.TableNo = i;

                SetupInfoChanged(this.setupInfo); //Notify that the setup info has changed.
            }
        }

        /// <summary>
        /// Writes the current stored setup information to the setup.conf file.
        /// </summary>
        public void WriteToConfig()
        {
            if (!File.Exists(Path))
            {
                this.ReadConfig(); //calling this method creates the config file.
            }
            string[] arr = new string[2];
            arr[0] = setupInfo.Ip;
            arr[1] = setupInfo.TableNo.ToString();
            File.WriteAllLines(Path, arr);
        }

        /// <summary>
        /// Updates the log with activity registered for the current voter
        /// </summary>
        /// <param name="ae">The activity to be logged</param>
        private void UpdateLog(ActivityEnum ae)
        {
            try
            {
                //Create the log DAO with setup information, activity and current voter. 
                var ldo = new LogDO(setupInfo.TableNo, currentVoter.PrimaryKey, ae);
                var ldao = new LogDAO(DigitalVoterList.GetInstanceFromServer(setupInfo.Ip));
                ldao.Create(ldo);
            }
            catch (Exception)
            {
                ConnectionError();
            }
        }

        #region Properties

        public SetupInfo SetupInfo
        {
            get { return this.setupInfo; }
            set { this.setupInfo = value; }
        }

        public string AdminPass
        {
            get { return this.adminPass; }
            set { this.adminPass = value; }
        }

        public static PessimisticVoterDAO StaticPvdao
        {
            get { return staticPvdao; }
            set { staticPvdao = value; }
        }

        #endregion
    }
}

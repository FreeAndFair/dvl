using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using DVLTerminal.GUI;
using DVLTerminal.Local;
using DVLTerminal.Networking;

namespace DVLTerminal
{
    static class Program
    {
        /// <summary>
        /// How many computer must each computer at least be connected to. 
        /// This means that the minimum amount of connected computers is MinimumPeerCount + 1
        /// </summary>
        public const int MinimumPeerCount = 2;

        /// <summary>
        /// The main entry point for the application. 
        /// Sets up a connection and starts the process of registration of voters
        /// </summary>
        [STAThread]
        static void Main()
        {
            Inbox.GetInstance().StartListening();
            
            var eventSystem = new EventSystem();
            var backgroundThread = new Thread(eventSystem.Start);
            var networkListener = new NetworkListener(100);
            eventSystem.RegisterEventSystemExecutable(networkListener);
            eventSystem.NotEnoughPeers += NotEnoughPeersHandler;
            backgroundThread.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var peers = SetUpConnection(networkListener);
            StartProgram(eventSystem, networkListener, peers);

            eventSystem.Stop();
        }

        /// <summary>
        /// Start the process of registrating voters
        /// </summary>
        /// <param name="eventSystem">The currently running event system</param>
        /// <param name="networkListener">The currently running networklistener</param>
        /// <param name="peers">A set of peers that are all running this program</param>
        private static void StartProgram(EventSystem eventSystem, NetworkListener networkListener, IEnumerable<IPAddress> peers)
        {
            var localVoteListener = new LocalVoteListener(10, networkListener, peers);

            networkListener.RegisterHandler(new PingListener());
            networkListener.RegisterHandler(new VoteListener(localVoteListener));
            networkListener.RegisterHandler(new PingRequestListener(networkListener));
            networkListener.RegisterHandler(new PeerDeadListener(localVoteListener));
            
            eventSystem.RegisterEventSystemExecutable(localVoteListener);

            var regWindow = new RegisterVoterWindow(localVoteListener);
            Application.Run(regWindow); //Blocking

        }

        /// <summary>
        /// Handles the case where there is not enough peers to continue execution
        /// </summary>
        /// <param name="numPeers">The remaining number of connected peers</param>
        private static void NotEnoughPeersHandler(int numPeers)
        {
            if (numPeers == 0)
            {
                MessageBox.Show(
                    "An error occurred.\n" +
                    "This computer is no longer connected to the network and can no longer be used for registration of voters.",
                    "Networking error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show(
                    "An error occurred.\n" +
                    "There is not enough computer connected to the network to continue" +
                    "normal operation. Number of computers left: " + numPeers,
                    "Networking error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            Environment.Exit(1);
        }

        /// <summary>
        /// Set up a connection to other computers running this program
        /// </summary>
        /// <param name="networkListener">The currently running NetworkListener</param>
        /// <returns>The discovered peers</returns>
        private static IEnumerable<IPAddress> SetUpConnection(NetworkListener networkListener)
        {
            var peers = new Dictionary<IPAddress, string>(); //IP -> Computer name
            bool tryAgain = true;
            while (tryAgain)
            {
                var connector = new Connector(peers);
                var connectionWindow = new ConnectionWindow(connector);

                networkListener.RegisterHandler(connector);

                Application.Run(connectionWindow); //Blocking
                var confWin = new ConfirmationWindow();

                confWin.WindowClosed += (res => tryAgain = !res);
                Application.Run(confWin); //Blocking

                networkListener.RemoveHandler(connector);
                connectionWindow.Dispose();
                confWin.Dispose();
            }
            peers.Remove(Inbox.GetInstance().MyIP);

            return from peer in peers select peer.Key;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using DVLTerminal.Networking;

namespace DVLTerminal.GUI
{
    /// <summary>
    /// The window that is first shown to the user, enabling them to connect to other computers
    /// </summary>
    public partial class ConnectionWindow : Form
    {
        /// <summary>
        /// The connector used to set up the connection
        /// </summary>
        private readonly Connector connector;

        /// <summary>
        /// A list of peers added to the UI list
        /// </summary>
        private readonly HashSet<IPAddress> addedPeers = new HashSet<IPAddress>();

        /// <summary>
        /// When false, the windows wont close under any circumstances
        /// </summary>
        private bool canClose;

        /// <summary>
        /// Create a new connection window with a given connector
        /// </summary>
        /// <param name="connector">The conenctor to use</param>
        public ConnectionWindow(Connector connector)
        {
            InitializeComponent();
            this.connector = connector;
        }

        /// <summary>
        /// When the window is loaded, broadcast a heartbeatrequest to ask who is present
        /// </summary>
        private void ConnectionWindow_Load(object sender, EventArgs e)
        {
            connector.BroadcastHeartbeatRequest();
        }

        /// <summary>
        /// Handle a click on the Connect button by asking who is present
        /// </summary>
        private void ConnectButtonClick(object sender, EventArgs e)
        {
            connector.BroadcastHeartbeatRequest();
        }

        /// <summary>
        /// Checks which peers is registered in the connector class. 
        /// All peers in the connector class who is not in the GUI list are added to the list
        /// </summary>
        private void NewPeerTimerTick(object sender, EventArgs e)
        {
            var newPeers = (from peer in connector.ConnectedPeers where !addedPeers.Contains(peer.Key) select peer.Key);

            foreach (var peer in newPeers)
            {
                addedPeers.Add(peer);
                var item = new ListViewItem(connector.ConnectedPeers[peer]);
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, peer.ToString()));
                listView1.Items.Add(item);
            }

            canClose = connector.ConnectionDone; //Close the window if a key packet is received
            if (canClose)
                Close();

            if (addedPeers.Count >= Program.MinimumPeerCount)
            {
                NextButton.Enabled = true;
            }
        }

        /// <summary>
        /// Handles click events from pressing the next button.
        /// Initiates key exchange and closes the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextButtonClick(object sender, EventArgs e)
        {
            NewPeerTimer.Stop();
            connector.SendKey();
            canClose = true;
            Close();
        }

        /// <summary>
        /// Disallow closing the form unless the connection has been made, or canceled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectionWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !canClose;
        }
    }
}

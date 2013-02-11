using System;

namespace DVLTerminal
{
    /// <summary>
    /// An exception thrown if there is not enough peers connected to continue operation
    /// </summary>
    public class NotEnoughPeersException : Exception
    {
        /// <summary>
        /// The number of connected peers at the time the exception occurred.
        /// </summary>
        public int NumPeers { get; private set; }

        /// <summary>
        /// Create a new NotEnoughPeers exception indicating that there is not enough connected computer to continue operation
        /// </summary>
        /// <param name="numPeers">The number of connected peers at the time the exception occurred</param>
        public NotEnoughPeersException(int numPeers)
        {
            NumPeers = numPeers;
        }
    }
}

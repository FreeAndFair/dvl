using System;
using System.Collections.Generic;
using System.Linq;

namespace DVLTerminal.Networking
{
    /// <summary>
    /// Listens for incoming network Messages and tries to delegate them to all registered handlers
    /// </summary>
    class NetworkListener : EventSystemExecutable
    {
        readonly Dictionary<INetworkPacketHandler, int> handlers = new Dictionary<INetworkPacketHandler, int>();

        public NetworkListener(int timeout) : base(timeout)
        {

        }

        /// <summary>
        /// Register a new handler to listen for network messages
        /// </summary>
        /// <param name="handler">The handler to register</param>
        /// <param name="timeout">The time in milliseoncds before the handler will timeout. If there should be no timeout, set as -1, or leave it out</param>
        public void RegisterHandler(INetworkPacketHandler handler, int timeout = -1)
        {
            lock (handlers)
            {
                if (timeout == -1)
                    handlers.Add(handler, 0);
                else
                    handlers.Add(handler, Environment.TickCount + timeout);
            }
        }

        /// <summary>
        /// Remove the specified handler so that it doesn't receive packets anymore and won't time out
        /// </summary>
        /// <param name="handler"></param>
        public void RemoveHandler(INetworkPacketHandler handler)
        {
            lock (handlers)
            {
                handlers.Remove(handler);
            }
        }

        /// <summary>
        /// Check the inbox for new messages and try to pass them to registered handlers
        /// </summary>
        private void CheckMessages()
        {
            if (!Inbox.GetInstance().HasMessages()) return; //Do nothing if there's no new messages
            Message message = Inbox.GetInstance().GetNextMessage();

            //Save the handling handler along with the response, to be able to remove it afterwards
            PacketResponse response = PacketResponse.Decline;
            INetworkPacketHandler handler = null;
            foreach (var currentHandler in handlers)
            {
                response = currentHandler.Key.HandlePacket(message);
                handler = currentHandler.Key;
                if (response != PacketResponse.Decline)
                {
                    break;
                }   
            }

            if (response == PacketResponse.AcceptAndFinish && handler != null)
                handlers.Remove(handler);

        }

        /// <summary>
        /// Check if any handlers has timed out. Invoke the TimedOut method and remove from handlers
        /// </summary>
        private void CheckTimeout()
        {
            var time = Environment.TickCount;
            var timedOut = (from handler in handlers where (handler.Value <= time && handler.Value != 0) select handler.Key).ToArray();
            
            foreach (var handler in timedOut)
            {
                handler.TimedOut();
                handlers.Remove(handler);
            }
        }

        /// <summary>
        /// Calls the CheckMessage and the CheckTimeout methods whenever the NetworkListener
        /// is running.
        /// </summary>
        protected override void Run()
        {
            lock (handlers)
            {
                CheckMessages();
                CheckTimeout();
            }
        }
    }
}

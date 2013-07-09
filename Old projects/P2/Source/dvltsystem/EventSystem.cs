using System.Collections.Generic;

namespace DVLTerminal
{
    /// <summary>
    /// An event simulations system for doing several tasks simultaneously
    /// </summary>
    class EventSystem
    {
        private bool stop;

        private readonly List<EventSystemExecutable> executables = new List<EventSystemExecutable>();

        /// <summary>
        /// A delegate capaple of handling an event where there are not enough peers connected
        /// to continue normal execution.
        /// </summary>
        /// <param name="numPeers">The remaining number of connected computer excluding this</param>
        public delegate void NotEnoughPeersHandler(int numPeers);

        /// <summary>
        /// Fired when a NotEnoughPeersException is raised somewhere in the program
        /// </summary>
        public event NotEnoughPeersHandler NotEnoughPeers;

        /// <summary>
        /// Register a new EventSystemExecutable to be run in the event system
        /// </summary>
        /// <param name="eventSystemExecutable">The new executable to register</param>
        public void RegisterEventSystemExecutable(EventSystemExecutable eventSystemExecutable)
        {
            executables.Add(eventSystemExecutable);
        }

        /// <summary>
        /// Stop the system releasing any thread stuck in "Start()"
        /// </summary>
        public void Stop()
        {
            stop = true;
        }

        /// <summary>
        /// Starts the entire system event loop
        /// </summary>
        public void Start()
        {
            stop = false;
            try
            {
                while (!stop)
                {
                    for (int i = 0; i < executables.Count; i++)
                        //The use of a 'for' loop allows for registration of Executables while running
                    {
                        executables[i].Start();
                    }
                }
            }
            catch (NotEnoughPeersException e)
            {
                if (NotEnoughPeers != null)
                    NotEnoughPeers(e.NumPeers);
            }
        }
    }
}

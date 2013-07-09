using System;

namespace DVLTerminal
{
    /// <summary>
    /// An executable part of an event system
    /// </summary>
    public abstract class EventSystemExecutable
    {
        private readonly int timeout;
        private int startTime;

        /// <summary>
        /// Create a new EventSystemExecutable with a given timeout
        /// </summary>
        /// <param name="timeout">For how long may this executable run in milliseconds</param>
        protected EventSystemExecutable(int timeout)
        {
            this.timeout = timeout;
        }

        /// <summary>
        /// Start running the executable until the timeout is reached
        /// </summary>
        public void Start()
        {
            startTime = Environment.TickCount;
            while (Environment.TickCount < (startTime + timeout))
            {
                Run();
            }
        }

        /// <summary>
        /// This method is called in a loop as long as this event is executing
        /// </summary>
        protected abstract void Run();
    }
}

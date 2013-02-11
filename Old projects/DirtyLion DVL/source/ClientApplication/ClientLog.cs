namespace ClientApplication {
    using System;
    using SmallTuba.Entities;

    /// <author>Christian Olsson (chro@itu.dk)</author>
    /// <version>2011-12-12</version>
    /// <summary>
    /// A log entry used for the log window
    /// </summary>
    public class ClientLog {
        /// <summary>
        /// The voter involved in this log instance
        /// </summary>
        private Person voter;

        /// <summary>
        /// The action performed by the client
        /// </summary>
        private string action;

        /// <summary>
        /// The time the action happened
        /// </summary>
        private DateTime time;

        /// <summary>
        /// Creates a new log post with this voter and this action
        /// </summary>
        /// <param name="voter">The voter</param>
        /// <param name="action">The action</param>
        public ClientLog(Person voter, string action) {
            this.voter = voter;
            this.action = action;
            this.time = DateTime.Now;
        }

        /// <summary>
        /// The voter involved in this log post
        /// </summary>
        public Person Voter {
            get { return this.voter; }
        }

        /// <summary>
        /// Returns this instance of a log state as: Hour:Minute FirstName LastName was Action
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            string hour = this.time.ToLocalTime().Hour.ToString();
            string minute;
            if (this.time.ToLocalTime().Minute < 10) {
                minute = "0" + this.time.ToLocalTime().Minute;
            }
            else{
                minute = this.time.ToLocalTime().Minute.ToString();
            }

            return hour + ":" + minute + " " + this.voter.FirstName + " " + this.voter.LastName + " was " + this.action;
        }
    }
}

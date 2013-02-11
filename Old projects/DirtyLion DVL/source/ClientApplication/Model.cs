namespace ClientApplication {
    using System.Collections.Generic;
    
    /// <author>Christian Olsson (chro@itu.dk)</author>
    /// <version>2011-12-12</version>
    /// <summary>
    /// The model of this application
    /// </summary>
    internal class Model {
        /// <summary>
        /// A log of who is registered at this client
        /// </summary>
        private List<ClientLog> log; 

        /// <summary>
        /// Creates a new model
        /// </summary>
        public Model() {
            this.log = new List<ClientLog>();
        }

        /// <summary>
        /// The name of the model
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The log of what has been registered at this client
        /// </summary>
        public List<ClientLog> Log {
            get { return this.log; }
        }
    }
}

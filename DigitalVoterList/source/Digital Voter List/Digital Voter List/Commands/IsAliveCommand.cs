﻿using System;
using System.Diagnostics.Contracts;
using System.Net;

namespace Aegis_DVL.Commands {
    [Serializable]
    public class IsAliveCommand : ICommand {

        /// <summary>
        /// May I have a new command that does nothing at the target?
        /// </summary>
        /// <param name="sender">The address of the one sending the command.</param>
        public IsAliveCommand(IPEndPoint sender) {
            Contract.Requires(sender != null);
            Sender = sender;
        }

        public IPEndPoint Sender { get; private set; }

        public void Execute(Station receiver) {
            //Do nothing
        }
    }
}
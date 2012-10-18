using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotiFire.SpotifyLib
{
    public class SessionEventArgs : EventArgs
    {
        Error error = Error.OK;
        string message = string.Empty;

        internal SessionEventArgs()
        {

        }

        internal SessionEventArgs(Error error)
        {
            this.error = error;
        }

        internal SessionEventArgs(string message)
        {
            this.message = message;
        }

        public Error Status
        {
            get
            {
                return this.error;
            }
        }

        public string Message
        {
            get
            {
                return this.message;
            }
        }

        public override string ToString()
        {
            return String.Format("SessionEvent:\n\tStatus: {0}\n\tMessage: {1}", Status, Message);
        }
    }
}

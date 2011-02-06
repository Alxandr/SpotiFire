using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotiFire.SpotifyLib
{
    public class SearchEventArgs : EventArgs
    {
        private Search result;
        private object state;

        internal SearchEventArgs(Search result, object state)
        {
            this.result = result;
            this.state = state;
        }

        public Search Result
        {
            get
            {
                return result;
            }
        }

        public object State
        {
            get
            {
                return state;
            }
        }
    }
}

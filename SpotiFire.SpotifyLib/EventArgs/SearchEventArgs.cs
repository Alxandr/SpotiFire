using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotiFire.SpotifyLib
{
    public class SearchEventArgs : EventArgs
    {
        private ISearch result;
        private object state;

        internal SearchEventArgs(ISearch result, object state)
        {
            this.result = result;
            this.state = state;
        }

        public ISearch Result
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

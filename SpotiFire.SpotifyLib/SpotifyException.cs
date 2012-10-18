using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotiFire.SpotifyLib
{
    public class SpotifyException : Exception
    {
        private Error res;

        public SpotifyException(Error res)
        {
            // TODO: Complete member initialization
            this.res = res;
        }

        public Error SpotifyError
        {
            get { return res; }
        }
    }
}

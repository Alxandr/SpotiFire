using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotiFire.SpotifyLib
{
    public class TracksEventArgs : EventArgs
    {
        int[] trackIndices;
        public TracksEventArgs(int[] trackIndices)
        {
            this.trackIndices = trackIndices;
        }
    }
}

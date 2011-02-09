﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotiFire.SpotifyLib
{
    public class TracksAddedEventArgs : TracksEventArgs
    {
        private ITrack[] tracks;
        public TracksAddedEventArgs(int[] trackIndices, ITrack[] tracks)
            : base(trackIndices)
        {
            this.tracks = tracks;
        }
    }
}

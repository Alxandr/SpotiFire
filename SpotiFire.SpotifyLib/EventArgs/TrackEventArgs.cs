using System;

namespace SpotiFire
{
    public class TrackEventArgs : EventArgs
    {
        private ITrack track;
        public TrackEventArgs(ITrack track)
        {
            this.track = track;
        }

        public ITrack Track
        {
            get { return track; }
        }

    }
}

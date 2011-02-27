using System;

namespace SpotiFire.SpotiClient
{
    public class PlaybackEventArgs : EventArgs
    {
        private ServiceReference.Track track;
        private Guid containerId;
        private int index;

        public PlaybackEventArgs(ServiceReference.Track track, Guid containerId, int index)
        {
            this.track = track;
            this.containerId = containerId;
            this.index = index;
        }

        public ServiceReference.Track Track
        {
            get { return track; }
        }
    }
}

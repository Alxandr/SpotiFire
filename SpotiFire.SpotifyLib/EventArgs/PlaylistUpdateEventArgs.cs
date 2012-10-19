using System;

namespace SpotiFire
{
    public class PlaylistUpdateEventArgs : EventArgs
    {
        private bool complete;
        public PlaylistUpdateEventArgs(bool complete)
        {
            this.complete = complete;
        }


        public bool Complete
        {
            get { return this.complete; }
        }

    }
}

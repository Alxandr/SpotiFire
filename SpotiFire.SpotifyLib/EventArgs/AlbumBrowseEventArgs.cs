using System;

namespace SpotiFire.SpotifyLib {
    public class AlbumBrowseEventArgs : EventArgs {
        private IAlbumBrowse result;
        private object state;

        internal AlbumBrowseEventArgs(IAlbumBrowse result, object state) {
            this.result = result;
            this.state = state;
        }

        public IAlbumBrowse Result {
            get { return result; }
        }

        public object State {
            get { return state; }
        }
    }
}

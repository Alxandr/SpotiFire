using System;

namespace SpotiFire.SpotifyLib {
	public class ArtistBrowseEventArgs : EventArgs {
        private IArtistBrowse result;
		private object state;

        internal ArtistBrowseEventArgs(IArtistBrowse result, object state) {
			this.result = result;
			this.state = state;
		}
		
		public IArtistBrowse Result {
			get { return result; }
		}
		
		public object State {
			get { return state; }
		}
	}
}

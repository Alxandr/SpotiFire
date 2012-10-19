using System;

namespace SpotiFire
{
    public class AlbumBrowseEventArgs : EventArgs
    {
        private IAlbumBrowse result;

        internal AlbumBrowseEventArgs(IAlbumBrowse result)
        {
            this.result = result;
        }

        public IAlbumBrowse Result
        {
            get { return result; }
        }
    }
}

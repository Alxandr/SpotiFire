using System;

namespace SpotiFire
{
    public class ImageEventArgs : EventArgs
    {
        string imageId;
        public ImageEventArgs(string imageId)
        {
            this.imageId = imageId;
        }
    }
}

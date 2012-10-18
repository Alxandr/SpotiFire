using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotiFire.SpotifyLib
{
    internal delegate void toplistbrowse_complete_delegate(IntPtr tlbPtr, IntPtr userDataPtr);

    /// <summary>
    /// Toplist types.
    /// </summary>
    public enum ToplistType
    {
        /// <summary>
        /// Top artists.
        /// </summary>
        Artists = 0,

        /// <summary>
        /// Top albums.
        /// </summary>
        Albums = 1,

        /// <summary>
        /// Top tracks.
        /// </summary>
        Tracks = 2,
    }

    /// <summary>
    /// Special toplist regions.
    /// </summary>
    public enum ToplistRegion
    {
        /// <summary>
        /// Global toplist.
        /// </summary>
        Everywhere = 0,

        /// <summary>
        /// Toplist for a given user.
        /// </summary>
        User = 1
    }

}

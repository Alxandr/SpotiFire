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

    static partial class libspotify
    {
        [DllImport("libspotify")]
        static internal extern IntPtr sp_toplistbrowse_create(IntPtr sessionPtr, ToplistType type, ToplistRegion region, string username, IntPtr callbackPtr, IntPtr userDataPtr);

        [DllImport("libspotify")]
        static internal extern bool sp_toplistbrowse_is_loaded(IntPtr tlbPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_toplistbrowse_error(IntPtr tlbPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_toplistbrowse_add_ref(IntPtr tlbPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_toplistbrowse_release(IntPtr tlbPtr);

        [DllImport("libspotify")]
        static internal extern int sp_toplistbrowse_num_artists(IntPtr tlbPtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_toplistbrowse_artist(IntPtr tlbPtr, int index);

        [DllImport("libspotify")]
        static internal extern int sp_toplistbrowse_num_albums(IntPtr tlbPtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_toplistbrowse_album(IntPtr tlbPtr, int index);

        [DllImport("libspotify")]
        static internal extern int sp_toplistbrowse_num_tracks(IntPtr tlbPtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_toplistbrowse_track(IntPtr tlbPtr, int index);

        [DllImport("libspotify")]
        static internal extern int sp_toplistbrowse_backend_request_duration(IntPtr tlbPtr);
    }
}

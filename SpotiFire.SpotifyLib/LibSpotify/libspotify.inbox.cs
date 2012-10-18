using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotiFire.SpotifyLib
{
    internal delegate void inboxpost_complete_delegate(IntPtr inboxPtr, IntPtr userDataPtr);

    static partial class libspotify
    {
        [DllImport("libspotify")]
        static internal extern IntPtr sp_inbox_post_tracks(IntPtr sessionPtr, string user, IntPtr trackPtrArray, int numTracks, string message, IntPtr callbackPtr, IntPtr userDataPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_inbox_error(IntPtr inboxPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_inbox_add_ref(IntPtr inboxPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_inbox_release(IntPtr inboxPtr);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotiFire.SpotifyLib
{
    internal delegate void albumbrowse_complete_delegate(IntPtr resultPtr, IntPtr userDataPtr);

    static partial class libspotify
    {
        [DllImport("libspotify")]
        static internal extern IntPtr sp_albumbrowse_create(IntPtr sessionPtr, IntPtr albumPtr, IntPtr callback, IntPtr userDataPtr);

        [DllImport("libspotify")]
        static internal extern bool sp_albumbrowse_is_loaded(IntPtr albPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_albumbrowse_error(IntPtr albPtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_albumbrowse_album(IntPtr albPtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_albumbrowse_artist(IntPtr albPtr);

        [DllImport("libspotify")]
        static internal extern int sp_albumbrowse_num_copyrights(IntPtr albPtr);

        [DllImport("libspotify")]
        static internal extern string sp_albumbrowse_copyright(IntPtr albPtr, int index);

        [DllImport("libspotify")]
        static internal extern int sp_albumbrowse_num_tracks(IntPtr albPtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_albumbrowse_track(IntPtr albPtr, int index);

        [DllImport("libspotify")]
        static internal extern string sp_albumbrowse_review(IntPtr albPtr);

        [DllImport("libspotify")]
        static internal extern int sp_albumbrowse_backend_request_duration(IntPtr albPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_albumbrowse_add_ref(IntPtr albPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_albumbrowse_release(IntPtr albPtr);
    }
}

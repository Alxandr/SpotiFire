using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotiFire.SpotifyLib
{
    static partial class libspotify
    {
        [DllImport("libspotify")]
        static internal extern bool sp_track_is_loaded(IntPtr trackPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_track_error(IntPtr trackPtr);

        [DllImport("libspotify")]
        static internal extern TrackOfflineStatus sp_track_offline_status(IntPtr trackPtr);

        [DllImport("libspotify")]
        static internal extern TrackAvailability sp_track_get_availability(IntPtr sessionPtr, IntPtr trackPtr);

        [DllImport("libspotify")]
        static internal extern bool sp_track_is_local(IntPtr sessionPtr, IntPtr trackPtr);

        [DllImport("libspotify")]
        static internal extern bool sp_track_is_autolinked(IntPtr sessionPtr, IntPtr trackPtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_track_get_playable(IntPtr sessionPtr, IntPtr trackPtr);

        [DllImport("libspotify")]
        static internal extern bool sp_track_is_placeholder(IntPtr trackPtr);

        //[DllImport("libspotify")]
        delegate IntPtr _sp_track_is_starred(IntPtr sessionPtr, IntPtr trackPtr);
        static internal bool sp_track_is_starred(IntPtr sessionPtr, IntPtr trackPtr)
        {
            var lib = LoadLibrary("libspotify.dll");
            var met = GetProcAddress(lib, "_sp_track_is_starred@8");

            var dlg = (_sp_track_is_starred)Marshal.GetDelegateForFunctionPointer(met, typeof(_sp_track_is_starred));
            var ret = dlg(sessionPtr, trackPtr);
            return ret != IntPtr.Zero;
        }

        [DllImport("libspotify")]
        static internal extern Error sp_track_set_starred(IntPtr sessionPtr, IntPtr trackPtrArrayPtr, int numTracks, bool start);

        [DllImport("libspotify")]
        static internal extern int sp_track_num_artists(IntPtr trackPtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_track_artist(IntPtr trackPtr, int index);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_track_album(IntPtr trackPtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_track_name(IntPtr trackPtr);

        [DllImport("libspotify")]
        static internal extern int sp_track_duration(IntPtr trackPtr);

        [DllImport("libspotify")]
        static internal extern int sp_track_popularity(IntPtr trackPtr);

        [DllImport("libspotify")]
        static internal extern int sp_track_disc(IntPtr trackPtr);

        [DllImport("libspotify")]
        static internal extern int sp_track_index(IntPtr trackPtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_localtrack_create(string artist, string title, string album, int length);

        [DllImport("libspotify")]
        static internal extern Error sp_track_add_ref(IntPtr trackPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_track_release(IntPtr trackPtr);
    }
}

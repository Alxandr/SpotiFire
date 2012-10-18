using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotiFire.SpotifyLib
{
    internal delegate void artistbrowse_complete_delegate(IntPtr resultPtr, IntPtr userDataPtr);

    static partial class libspotify
    {
        [DllImport("libspotify")]
        static internal extern IntPtr sp_artistbrowse_create(IntPtr sessionPtr, IntPtr artistPtr, ArtistBrowseType type, IntPtr callback, IntPtr userDataPtr);

        [DllImport("libspotify")]
        static internal extern bool sp_artistbrowse_is_loaded(IntPtr arbPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_artistbrowse_error(IntPtr arbPtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_artistbrowse_artist(IntPtr arbPtr);

        [DllImport("libspotify")]
        static internal extern int sp_artistbrowse_num_portraits(IntPtr arbPtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_artistbrowse_portrait(IntPtr arbPtr, int index);

        [DllImport("libspotify")]
        static internal extern int sp_artistbrowse_num_tracks(IntPtr arbPtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_artistbrowse_track(IntPtr arbPtr, int index);

        [DllImport("libspotify")]
        static internal extern int sp_artistbrowse_num_tophit_tracks(IntPtr arbPtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_artistbrowse_tophit_track(IntPtr arbPtr, int index);

        [DllImport("libspotify")]
        static internal extern int sp_artistbrowse_num_albums(IntPtr arbPtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_artistbrowse_album(IntPtr arbPtr, int index);

        [DllImport("libspotify")]
        static internal extern int sp_artistbrowse_num_similar_artists(IntPtr arbPtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_artistbrowse_similar_artist(IntPtr arbPtr, int index);

        [DllImport("libspotify")]
        static internal extern string sp_artistbrowse_biography(IntPtr arbPtr);

        [DllImport("libspotify")]
        static internal extern int sp_artistbrowse_backend_request_duration(IntPtr arbPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_artistbrowse_add_ref(IntPtr arbPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_artistbrowse_release(IntPtr arbPtr);
    }
}

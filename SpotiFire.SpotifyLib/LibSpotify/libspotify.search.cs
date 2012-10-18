using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotiFire.SpotifyLib
{
    internal delegate void search_complete_delegate(IntPtr resultPtr, IntPtr userDataPtr);

    static partial class libspotify
    {
        [DllImport("libspotify")]
        static internal extern IntPtr sp_search_create(IntPtr sessionPtr, string query, int trackOffset, int trackCount, int albumOffset, int albumCount, int artistOffset, int artistCount, int playlistOffset, int playlistCount, SearchType type, IntPtr callbackPtr, IntPtr userDataPtr);

        [DllImport("libspotify")]
        static internal extern bool sp_search_is_loaded(IntPtr searchPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_search_error(IntPtr searchPtr);

        [DllImport("libspotify")]
        static internal extern int sp_search_num_tracks(IntPtr searchPtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_search_track(IntPtr searchPtr, int index);

        [DllImport("libspotify")]
        static internal extern int sp_search_num_albums(IntPtr searchPtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_search_album(IntPtr searchPtr, int index);

        [DllImport("libspotify")]
        static internal extern int sp_search_num_playlists(IntPtr searchPtr);

        [DllImport("libspotify")]
        static internal extern string sp_search_playlist_name(IntPtr searchPtr, int index);

        [DllImport("libspotify")]
        static internal extern string sp_search_playlist_uri(IntPtr searchPtr, int index);

        [DllImport("libspotify")]
        static internal extern string sp_search_playlist_image_uri(IntPtr searchPtr, int index);

        [DllImport("libspotify")]
        static internal extern int sp_search_num_artists(IntPtr searchPtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_search_artist(IntPtr searchPtr, int index);

        [DllImport("libspotify")]
        static internal extern string sp_search_query(IntPtr searchPtr);

        [DllImport("libspotify")]
        static internal extern string sp_search_did_you_mean(IntPtr searchPtr);

        [DllImport("libspotify")]
        static internal extern int sp_search_total_tracks(IntPtr searchPtr);

        [DllImport("libspotify")]
        static internal extern int sp_search_total_albums(IntPtr searchPtr);

        [DllImport("libspotify")]
        static internal extern int sp_search_total_artists(IntPtr searchPtr);

        [DllImport("libspotify")]
        static internal extern int sp_search_total_playlists(IntPtr searchPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_search_add_ref(IntPtr searchPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_search_release(IntPtr searchPtr);
    }
}

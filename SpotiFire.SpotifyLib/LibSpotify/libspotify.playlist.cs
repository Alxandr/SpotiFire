using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotiFire.SpotifyLib
{
    internal delegate void tracks_added_delegate(IntPtr playlistPtr, IntPtr trackPtrArray, int numTracks, int position, IntPtr userDataPtr);
    internal delegate void tracks_removed_delegate(IntPtr playlistPtr, IntPtr trackIndexArray, int numTracks, IntPtr userDataPtr);
    internal delegate void tracks_moved_delegate(IntPtr playlistPtr, IntPtr trackIndexArray, int numTracks, int newPosition, IntPtr userDataPtr);
    internal delegate void playlist_renamed_delegate(IntPtr playlistPtr, IntPtr userDataPtr);
    internal delegate void playlist_state_changed_delegate(IntPtr playlistPtr, IntPtr userDataPtr);
    internal delegate void playlist_update_in_progress_delegate(IntPtr playlistPtr, bool done, IntPtr userDataPtr);
    internal delegate void playlist_metadata_updated_delegate(IntPtr playlistPtr, IntPtr userDataPtr);
    internal delegate void track_created_changed_delegate(IntPtr playlistPtr, int position, IntPtr userPtr, int when, IntPtr userDataPtr);
    internal delegate void track_seen_changed_delegate(IntPtr playlistPtr, int position, bool seen, IntPtr userDataPtr);
    internal delegate void description_changed_delegate(IntPtr playlistPtr, string desc, IntPtr userDataPtr);
    internal delegate void image_changed_delegate(IntPtr playlistPtr, IntPtr imagePtr/*ID?*/, IntPtr userDataPtr);
    internal delegate void track_message_changed_delegate(IntPtr playlistPtr, int position, string message, IntPtr userDataPtr);
    internal delegate void subscribers_changed_delegate(IntPtr playlistPtr, IntPtr userDataPtr);

    [StructLayout(LayoutKind.Sequential)]
    internal struct PlaylistCallbacks
    {
        internal IntPtr tracks_added;
        internal IntPtr tracks_removed;
        internal IntPtr tracks_moved;
        internal IntPtr playlist_renamed;
        internal IntPtr playlist_state_changed;
        internal IntPtr playlist_update_in_progress;
        internal IntPtr playlist_metadata_updated;
        internal IntPtr track_created_changed;
        internal IntPtr track_seen_changed;
        internal IntPtr description_changed;
        internal IntPtr image_changed;
        internal IntPtr track_message_changed;
        internal IntPtr subscribers_changed;
    }

    static partial class libspotify
    {
        [DllImport("libspotify")]
        static internal extern bool sp_playlist_is_loaded(IntPtr plPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_playlist_add_callbacks(IntPtr plPtr, ref PlaylistCallbacks callbacks, IntPtr userDataPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_playlist_remove_callbacks(IntPtr plPtr, ref PlaylistCallbacks callbacks, IntPtr userDataPtr);

        [DllImport("libspotify")]
        static internal extern int sp_playlist_num_tracks(IntPtr plPtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_playlist_track(IntPtr plPtr, int index);

        [DllImport("libspotify")]
        static internal extern int sp_playlist_track_create_time(IntPtr plPtr, int index);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_playlist_track_creator(IntPtr plPtr, int index);

        [DllImport("libspotify")]
        static internal extern bool sp_playlist_track_seen(IntPtr plPtr, int index);

        [DllImport("libspotify")]
        static internal extern Error sp_playlist_track_set_seen(IntPtr plPtr, int index, bool seen);

        [DllImport("libspotify")]
        static internal extern string sp_playlist_track_message(IntPtr plPtr, int index);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_playlist_name(IntPtr plPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_playlist_rename(IntPtr plPtr, string newName);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_playlist_owner(IntPtr plPtr);

        [DllImport("libspotify")]
        static internal extern bool sp_playlist_is_collaborative(IntPtr plPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_playlist_set_collaborative(IntPtr plPtr, bool collaborative);

        [DllImport("libspotify")]
        static internal extern Error sp_playlist_set_autolink_tracks(IntPtr plPtr, bool autolink);

        [DllImport("libspotify")]
        static internal extern string sp_playlist_get_description(IntPtr plPtr);

        [DllImport("libspotify")]
        static internal extern bool sp_playlist_get_image(IntPtr plPtr, IntPtr imageIdPtr);

        [DllImport("libspotify")]
        static internal extern bool sp_playlist_has_pending_changes(IntPtr plPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_playlist_add_tracks(IntPtr plPtr, IntPtr trackPtrArray, int numTracks, int position, IntPtr sessionPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_playlist_remove_tracks(IntPtr plPtr, IntPtr trackIndexArray, int numTracks);

        [DllImport("libspotify")]
        static internal extern Error sp_playlist_reorder_tracks(IntPtr plPtr, IntPtr trackIndexArray, int numTracks, int newPosition);

        [DllImport("libspotify")]
        static internal extern uint sp_playlist_num_subscribers(IntPtr plPtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_playlist_subscribers(IntPtr plPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_playlist_subscribers_free(IntPtr subsPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_playlist_update_subscribers(IntPtr sessionPtr, IntPtr plPtr);

        [DllImport("libspotify")]
        static internal extern bool sp_playlist_is_in_ram(IntPtr sessionPtr, IntPtr plPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_playlist_set_in_ram(IntPtr sessionPtr, IntPtr plPtr, bool inRam);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_playlist_create(IntPtr sessionPtr, IntPtr linkPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_playlist_set_offline_mode(IntPtr sessionPtr, IntPtr plPtr, bool offline);

        [DllImport("libspotify")]
        static internal extern PlaylistOfflineStatus sp_playlist_get_offline_status(IntPtr sessionPtr, IntPtr plPtr);

        [DllImport("libspotify")]
        static internal extern int sp_playlist_get_offline_download_completed(IntPtr sessionPtr, IntPtr plPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_playlist_add_ref(IntPtr plPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_playlist_release(IntPtr plPtr);
    }
}

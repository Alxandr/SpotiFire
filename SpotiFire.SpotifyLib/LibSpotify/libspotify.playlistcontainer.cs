using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotiFire.SpotifyLib
{
    internal delegate void playlist_added_delegate(IntPtr pcPtr, IntPtr plPtr, int position, IntPtr userDataPtr);
    internal delegate void playlist_removed_delegate(IntPtr pcPtr, IntPtr plPtr, int position, IntPtr userDataPtr);
    internal delegate void playlist_moved_delegate(IntPtr pcPtr, IntPtr plPtr, int position, int newPosition, IntPtr userDataPtr);
    internal delegate void container_loaded_delegate(IntPtr playlistPtr, IntPtr userDataPtr);

    [StructLayout(LayoutKind.Sequential)]
    internal struct PlaylistContainerCallbacks
    {
        internal IntPtr playlist_added;
        internal IntPtr playlist_removed;
        internal IntPtr playlist_moved;
        internal IntPtr container_loaded;
    }

    static partial class libspotify
    {
        [DllImport("libspotify")]
        static internal extern Error sp_playlistcontainer_add_callbacks(IntPtr pcPtr, ref PlaylistContainerCallbacks callbacks, IntPtr userDataPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_playlistcontainer_remove_callbacks(IntPtr pcPtr, ref PlaylistContainerCallbacks callbacks, IntPtr userDataPtr);

        [DllImport("libspotify")]
        static internal extern int sp_playlistcontainer_num_playlists(IntPtr pcPtr);

        [DllImport("libspotify")]
        static internal extern bool sp_playlistcontainer_is_loaded(IntPtr pcPtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_playlistcontainer_playlist(IntPtr pcPtr, int index);

        [DllImport("libspotify")]
        static internal extern PlaylistType sp_playlistcontainer_playlist_type(IntPtr pcPtr, int index);

        [DllImport("libspotify")]
        static internal extern Error sp_playlistcontainer_playlist_folder_name(IntPtr pcPtr, int index, IntPtr bufferPtr, int bufferSize);

        [DllImport("libspotify")]
        static internal extern ulong sp_playlistcontainer_playlist_folder_id(IntPtr pcPtr, int index);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_playlistcontainer_add_new_playlist(IntPtr pcPtr, string name);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_playlistcontainer_add_playlist(IntPtr pcPtr, IntPtr linkPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_playlistcontainer_remove_playlist(IntPtr pcPter, int index);

        /// <summary>
        /// Move a playlist in the playlist container.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="index">Index of playlist to be moved.</param>
        /// <param name="newPosition">New position for the playlist.</param>
        /// <param name="dryRun">Do not execute the move, only check if it possible.</param>
        /// <returns>One of the following errors, from sp_error SP_ERROR_OK SP_ERROR_INDEX_OUT_OF_RANGE SP_ERROR_INVALID_INDATA - If trying to move a folder into itself.</returns>
        [DllImport("libspotify")]
        static internal extern Error sp_playlistcontainer_move_playlist(IntPtr pcPtr, int index, int newPosition, bool dryRun);

        [DllImport("libspotify")]
        static internal extern Error sp_playlistcontainer_add_folder(IntPtr pcPtr, int index, string name);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_playlistcontainer_owner(IntPtr pcPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_playlistcontainer_add_ref(IntPtr pcPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_playlistcontainer_release(IntPtr pcPtr);

        /// <summary>
        /// Get the number of new tracks in a playlist since the corresponding function sp_playlistcontainer_clear_unseen_tracks() was called.
        /// The function always returns the number of new tracks, and fills the tracks array with the new tracks, but not more than specified in num_tracks.
        /// The function will return a negative value on failure.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="plPtr">Playlist object.</param>
        /// <param name="tracksPtrArray">Array of pointer to new tracks (may be NULL).</param>
        /// <param name="numTracks">Size of tracks array.</param>
        /// <returns>Returns the number of unseen tracks.</returns>
        [DllImport("libspotify")]
        static internal extern int sp_playlistcontainer_get_unseen_tracks(IntPtr pcPtr, IntPtr plPtr, IntPtr trackPtrArray, int numTracks);

        [DllImport("libspotify")]
        static internal extern int sp_playlistcontainer_clear_unseen_tracks(IntPtr pcPtr, IntPtr plPtr);
    }
}

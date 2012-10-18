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
}

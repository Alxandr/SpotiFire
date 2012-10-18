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
}

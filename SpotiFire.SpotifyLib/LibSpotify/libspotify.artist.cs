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
        static internal extern IntPtr sp_artist_name(IntPtr artistPtr);

        [DllImport("libspotify")]
        static internal extern bool sp_artist_is_loaded(IntPtr artistPtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_artist_portrait(IntPtr artistPtr, ImageSize size);

        [DllImport("libspotify")]
        static internal extern Error sp_artist_add_ref(IntPtr artistPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_artist_release(IntPtr artistPtr);
    }
}

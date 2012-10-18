using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotiFire.SpotifyLib
{
    public enum AlbumType
    {
        Album = 0,
        Single = 1,
        Compilation = 2,
        Unknown = 3
    }

    static partial class libspotify
    {
        [DllImport("libspotify")]
        static internal extern bool sp_album_is_loaded(IntPtr albumPtr);

        [DllImport("libspotify")]
        static internal extern bool sp_album_is_available(IntPtr albumPtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_album_artist(IntPtr albumPtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_album_cover(IntPtr albumPtr, ImageSize size);

        [DllImport("libspotify")]
        static internal extern string sp_album_name(IntPtr albumPtr);

        [DllImport("libspotify")]
        static internal extern int sp_album_year(IntPtr albumPtr);

        [DllImport("libspotify")]
        static internal extern AlbumType sp_album_type(IntPtr albumPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_album_add_ref(IntPtr albumPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_album_release(IntPtr albumPtr);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotiFire.SpotifyLib
{
    internal delegate void image_loaded_delegate(IntPtr imagePtr, IntPtr userDataPtr);

    /// <summary>
    /// Image format.
    /// </summary>
    public enum ImageFormat
    {
        /// <summary>
        /// Unknown image format.
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// JPEG image.
        /// </summary>
        JPEG = 0,
    }

    static partial class libspotify
    {
        [DllImport("libspotify")]
        static internal extern IntPtr sp_image_create(IntPtr sessionPtr, IntPtr imageIdArr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_image_create_from_link(IntPtr sessionPtr, IntPtr linkPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_image_add_load_callback(IntPtr imagePtr, IntPtr callbackPtr, IntPtr userDataPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_image_remove_load_callback(IntPtr imagePtr, IntPtr callbackPtr, IntPtr userDataPtr);

        [DllImport("libspotify")]
        static internal extern bool sp_image_is_loaded(IntPtr imagePtr);

        [DllImport("libspotify")]
        static internal extern Error sp_image_error(IntPtr imagePtr);

        [DllImport("libspotify")]
        static internal extern ImageFormat sp_image_format(IntPtr imagePtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_image_data(IntPtr imagePtr, out int dataSize);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_image_image_id(IntPtr imagePtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_image_add_ref(IntPtr imagePtr);

        [DllImport("libspotify")]
        static internal extern IntPtr sp_image_release(IntPtr imagePtr);
    }
}

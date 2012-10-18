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
}

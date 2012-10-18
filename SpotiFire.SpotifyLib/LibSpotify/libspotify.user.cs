using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotiFire.SpotifyLib
{
    /// <summary>
    /// User relation type.
    /// </summary>
    public enum RelationType
    {
        /// <summary>
        /// Not yet known.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// No relation.
        /// </summary>
        None = 1,

        /// <summary>
        /// The currently logged in user is following this user.
        /// </summary>
        Unidirectional = 2,

        /// <summary>
        /// Bidirectional friendship established.
        /// </summary>
        Bidirectional = 3,
    }

    static partial class libspotify
    {
        [DllImport("libspotify")]
        static internal extern string sp_user_canonical_name(IntPtr userPtr);

        [DllImport("libspotify")]
        static internal extern string sp_user_display_name(IntPtr userPtr);

        [DllImport("libspotify")]
        static internal extern bool sp_user_is_loaded(IntPtr userPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_user_add_ref(IntPtr userPtr);

        [DllImport("libspotify")]
        static internal extern Error sp_user_release(IntPtr userPtr);
    }
}

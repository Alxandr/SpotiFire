using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotiFire.SpotifyLib
{
    /// <summary>
    /// Link types.
    /// </summary>
    public enum LinkType
    {
        /// <summary>
        /// Link type not valid - default until the library has parsed the link, or when parsing failed.
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// Link is Track.
        /// </summary>
        Track = 1,

        /// <summary>
        /// Link is Album.
        /// </summary>
        Album = 2,

        /// <summary>
        /// Link is Artist.
        /// </summary>
        Artist = 3,

        /// <summary>
        /// Link is Search.
        /// </summary>
        Search = 4,

        /// <summary>
        /// Link is Playlist.
        /// </summary>
        Playlist = 5,

        /// <summary>
        /// Link is Profile.
        /// </summary>
        Profile = 6,

        /// <summary>
        /// Link is Starred.
        /// </summary>
        Starred = 7,

        /// <summary>
        /// Link is Localtrack.
        /// </summary>
        Localtrack = 8,

        /// <summary>
        /// Link is Image.
        /// </summary>
        Image = 10,
    }
}

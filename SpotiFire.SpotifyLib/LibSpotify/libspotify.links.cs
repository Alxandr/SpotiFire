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

    static partial class libspotify
    {
        /// <summary>
        /// Create a Spotify link given a string.
        /// </summary>
        /// <param name="link">A string representation of a Spotify link.</param>
        /// <returns>A link representation of the given string representation. If the link could not be parsed, this function returns NULL.</returns>
        [DllImport("libspotify")]
        static internal extern IntPtr sp_link_create_from_string(string link);

        /// <summary>
        /// Generates a link object from a track.
        /// </summary>
        /// <param name="trackPtr">A track object.</param>
        /// <param name="offset">Offset in track in ms.</param>
        /// <returns>A link representing the track.</returns>
        [DllImport("libspotify")]
        static internal extern IntPtr sp_link_create_from_track(IntPtr trackPtr, int offset);

        /// <summary>
        /// Create a link object from an album.
        /// </summary>
        /// <param name="albumPtr">An album object.</param>
        /// <returns>A link representing the album.</returns>
        [DllImport("libspotify")]
        static internal extern IntPtr sp_link_create_from_album(IntPtr albumPtr);

        /// <summary>
        /// Create an image link object from an album.
        /// </summary>
        /// <param name="albumPtr">An album object.</param>
        /// <param name="size">The desired size of the image.</param>
        /// <returns>A link representing the album cover. Type is set to SP_LINKTYPE_IMAGE.</returns>
        [DllImport("libspotify")]
        static internal extern IntPtr sp_link_create_from_album_cover(IntPtr albumPtr, ImageSize size);

        /// <summary>
        /// Create a link object from an artist.
        /// </summary>
        /// <param name="artistPtr">An artist object.</param>
        /// <returns>A link representing the artist.</returns>
        [DllImport("libspotify")]
        static internal extern IntPtr sp_link_create_from_artist(IntPtr artistPtr);

        /// <summary>
        /// Creates a link object pointing to an artist portrait.
        /// </summary>
        /// <param name="artistPtr">Artist browse object.</param>
        /// <param name="size">The desired size of the image.</param>
        /// <returns>A link object representing an image.</returns>
        [DllImport("libspotify")]
        static internal extern IntPtr sp_link_create_from_artist_portrait(IntPtr artistPtr, ImageSize size);

        /// <summary>
        /// Creates a link object from an artist portrait.
        /// </summary>
        /// <param name="artistPtr">Artist browse object.</param>
        /// <param name="index">The index of the portrait. Should be in the interval [0, sp_artistbrowse_num_portraits() - 1].</param>
        /// <returns>A link object representing an image.</returns>
        [DllImport("libspotify")]
        static internal extern IntPtr sp_link_create_from_artistbrowse_portrait(IntPtr artistPtr, int index);

        /// <summary>
        /// Generate a link object representing the current search.
        /// </summary>
        /// <param name="searchPtr">Search object.</param>
        /// <returns>A link representing the search.</returns>
        [DllImport("libspotify")]
        static internal extern IntPtr sp_link_create_from_search(IntPtr searchPtr);

        /// <summary>
        /// Create a link object representing the given playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <returns>A link representing the playlist.</returns>
        [DllImport("libspotify")]
        static internal extern IntPtr sp_link_create_from_playlist(IntPtr playlistPtr);

        /// <summary>
        /// Create a link object representing the given playlist.
        /// </summary>
        /// <param name="userPtr">User object.</param>
        /// <returns>A link representing the profile.</returns>
        [DllImport("libspotify")]
        static internal extern IntPtr sp_link_create_from_user(IntPtr userPtr);

        /// <summary>
        /// Create a link object representing the given image.
        /// </summary>
        /// <param name="imagePtr">Image object.</param>
        /// <returns>A link representing the image.</returns>
        [DllImport("libspotify")]
        static internal extern IntPtr sp_link_create_from_image(IntPtr imagePtr);

        /// <summary>
        /// Create a string representation of the given Spotify link.
        /// </summary>
        /// <param name="linkPtr">The Spotify link whose string representation you are interested in.</param>
        /// <param name="bufferPtr">The buffer to hold the string representation of link.</param>
        /// <param name="bufferSize">The max size of the buffer that will hold the string representation The resulting string is guaranteed to always be null terminated if buffer_size > 0.</param>
        /// <returns>The number of characters in the string representation of the link. If this value is greater or equal than buffer_size, output was truncated.</returns>
        [DllImport("libspotify")]
        static internal extern int sp_link_as_string(IntPtr linkPtr, IntPtr bufferPtr, int bufferSize);

        /// <summary>
        /// The link type of the specified link.
        /// </summary>
        /// <param name="linkPtr">The Spotify link whose type you are interested in.</param>
        /// <returns>The link type of the specified link - see the sp_linktype enum for possible values</returns>
        [DllImport("libspotify")]
        static internal extern LinkType sp_link_type(IntPtr linkPtr);

        /// <summary>
        /// The track representation for the given link.
        /// </summary>
        /// <param name="linkPtr">The Spotify link whose track you are interested in.</param>
        /// <returns>The track representation of the given track link If the link is not of track type then NULL is returned.</returns>
        [DllImport("libspotify")]
        static internal extern IntPtr sp_link_as_track(IntPtr linkPtr);

        /// <summary>
        /// The track and offset into track representation for the given link.
        /// </summary>
        /// <param name="linkPtr">The Spotify link whose track you are interested in.</param>
        /// <param name="offset">Pointer to offset into track (in milliseconds). If the link does not contain an offset this will be set to 0.</param>
        /// <returns>The track representation of the given track link If the link is not of track type then NULL is returned.</returns>
        [DllImport("libspotify")]
        static internal extern IntPtr sp_link_as_track_and_offset(IntPtr linkPtr, out int offset);

        /// <summary>
        /// The album representation for the given link.
        /// </summary>
        /// <param name="linkPtr">The Spotify link whose album you are interested in</param>
        /// <returns>The album representation of the given album link If the link is not of album type then NULL is returned.</returns>
        [DllImport("libspotify")]
        static internal extern IntPtr sp_link_as_album(IntPtr linkPtr);

        /// <summary>
        /// The artist representation for the given link.
        /// </summary>
        /// <param name="linkPtr">The Spotify link whose artist you are interested in.</param>
        /// <returns>The artist representation of the given link If the link is not of artist type then NULL is returned.</returns>
        [DllImport("libspotify")]
        static internal extern IntPtr sp_link_as_artist(IntPtr linkPtr);

        /// <summary>
        /// The user representation for the given link.
        /// </summary>
        /// <param name="linkPtr">The Spotify link whose user you are interested in.</param>
        /// <returns>The user representation of the given link If the link is not of user type then NULL is returned.</returns>
        [DllImport("libspotify")]
        static internal extern IntPtr sp_link_as_user(IntPtr linkPtr);

        /// <summary>
        /// Increase the reference count of a link.
        /// </summary>
        /// <param name="linkPtr">The link object.</param>
        /// <returns>One of the following errors, from Error Error_OK.</returns>
        [DllImport("libspotify")]
        static internal extern Error sp_link_add_ref(IntPtr linkPtr);

        /// <summary>
        /// Create a link object representing the given playlist.
        /// </summary>
        /// <param name="linkPtr">User object.</param>
        /// <returns>A link representing the profile.</returns>
        [DllImport("libspotify")]
        static internal extern Error sp_link_release(IntPtr linkPtr);
    }
}

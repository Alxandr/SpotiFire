using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotiFire.SpotifyLib
{
    /// <summary>
    /// Error codes returned by various functions
    /// </summary>
    public enum Error
    {
        /// <summary>
        /// No errors encountered
        /// </summary>
        OK = 0,

        /// <summary>
        /// The library version targeted does not match the one you claim you support
        /// </summary>
        BAD_API_VERSION = 1,

        /// <summary>
        /// Initialization of library failed - are cache locations etc. valid?
        /// </summary>
        API_INITIALIZATION_FAILED = 2,

        /// <summary>
        /// The track specified for playing cannot be played
        /// </summary>
        TRACK_NOT_PLAYABLE = 3,

        /// <summary>
        /// The application key is invalid
        /// </summary>
        BAD_APPLICATION_KEY = 5,

        /// <summary>
        /// Login failed because of bad username and/or password
        /// </summary>
        BAD_USERNAME_OR_PASSWORD = 6,

        /// <summary>
        /// The specified username is banned
        /// </summary>
        USER_BANNED = 7,

        /// <summary>
        /// Cannot connect to the Spotify backend system
        /// </summary>
        UNABLE_TO_CONTACT_SERVER = 8,

        /// <summary>
        /// Client is too old, library will need to be updated
        /// </summary>
        CLIENT_TOO_OLD = 9,

        /// <summary>
        /// Some other error occurred, and it is permanent (e.g. trying to relogin will not help)
        /// </summary>
        OTHER_PERMANENT = 10,

        /// <summary>
        /// The user agent string is invalid or too long
        /// </summary>
        BAD_USER_AGENT = 11,

        /// <summary>
        /// No valid callback registered to handle events
        /// </summary>
        MISSING_CALLBACK = 12,

        /// <summary>
        /// Input data was either missing or invalid
        /// </summary>
        INVALID_INDATA = 13,

        /// <summary>
        /// Index out of range
        /// </summary>
        INDEX_OUT_OF_RANGE = 14,

        /// <summary>
        /// The specified user needs a premium account
        /// </summary>
        USER_NEEDS_PREMIUM = 15,

        /// <summary>
        /// A transient error occurred.
        /// </summary>
        OTHER_TRANSIENT = 16,

        /// <summary>
        /// The resource is currently loading
        /// </summary>
        IS_LOADING = 17,

        /// <summary>
        /// Could not find any suitable stream to play
        /// </summary>
        NO_STREAM_AVAILABLE = 18,

        /// <summary>
        /// Requested operation is not allowed
        /// </summary>
        PERMISSION_DENIED = 19,

        /// <summary>
        /// Target inbox is full
        /// </summary>
        INBOX_IS_FULL = 20,

        /// <summary>
        /// Cache is not enabled
        /// </summary>
        NO_CACHE = 21,

        /// <summary>
        /// Requested user does not exist
        /// </summary>
        NO_SUCH_USER = 22,

        /// <summary>
        /// No credentials are stored
        /// </summary>
        NO_CREDENTIALS = 23,

        /// <summary>
        /// Network disabled
        /// </summary>
        NETWORK_DISABLED = 24,

        /// <summary>
        /// Invalid device ID
        /// </summary>
        INVALID_DEVICE_ID = 25,

        /// <summary>
        /// Unable to open trace file
        /// </summary>
        CANT_OPEN_TRACE_FILE = 26,

        /// <summary>
        /// This application is no longer allowed to use the Spotify service
        /// </summary>
        APPLICATION_BANNED = 27,

        /// <summary>
        /// Reached the device limit for number of tracks to download
        /// </summary>
        OFFLINE_TOO_MANY_TRACKS = 31,

        /// <summary>
        /// Disk cache is full so no more tracks can be downloaded to offline mode
        /// </summary>
        OFFLINE_DISK_CACHE = 32,

        /// <summary>
        /// Offline key has expired, the user needs to go online again
        /// </summary>
        OFFLINE_EXPIRED = 33,

        /// <summary>
        /// This user is not allowed to use offline mode
        /// </summary>
        OFFLINE_NOT_ALLOWED = 34,

        /// <summary>
        /// The license for this device has been lost. Most likely because the user used offline on three other device
        /// </summary>
        OFFLINE_LICENSE_LOST = 35,

        /// <summary>
        /// The Spotify license server does not respond correctly
        /// </summary>
        OFFLINE_LICENSE_ERROR = 36,

        /// <summary>
        /// A LastFM scrobble authentication error has occurred
        /// </summary>
        LASTFM_AUTH_ERROR = 39,

        /// <summary>
        /// An invalid argument was specified
        /// </summary>
        INVALID_ARGUMENT = 40,

        /// <summary>
        /// An operating system error
        /// </summary>
        SYSTEM_FAILURE = 41,
    }

    static partial class libspotify
    {
        /// <summary>
        /// Convert a numeric libspotify error code to a text string.
        /// The error message is in English. This function is useful for logging purposes.
        /// </summary>
        /// <param name="error">The error code.</param>
        /// <returns>The text-representation of the error.</returns>
        [DllImport("libspotify")]
        public static extern string Error_message(Error error);
    }
}

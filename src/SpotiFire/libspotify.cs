using System;
using System.Runtime.InteropServices;

namespace SpotiFire
{
    public class libspotify
    {
        internal const int SPOTIFY_API_VERSION = 12;

        internal static readonly object mutex = new object();

        #region Error Handling
        [DllImport("libspotify")]
        internal static extern IntPtr sp_error_message(sp_error error);
        #endregion

        #region Session Handling
        #region Delegate
        internal delegate void session_callback(IntPtr session);
        internal delegate void logged_in(IntPtr session, sp_error result);
        #endregion

        [DllImport("libspotify")]
        internal static extern sp_error sp_session_create(IntPtr config, out IntPtr session);

        [DllImport("libspotify")]
        internal static extern sp_error sp_session_release(IntPtr session);

        [DllImport("libspotify")]
        internal static extern sp_error sp_session_login(IntPtr session, IntPtr username, IntPtr password, bool remember_me, IntPtr blob);

        [DllImport("libspotify")]
        internal static extern sp_error sp_session_process_events(IntPtr session, out int nextTimeout);
        #endregion
    }

    internal struct sp_session_config
    {
        internal int api_version;
        internal IntPtr cache_location;
        internal IntPtr settings_location;
        internal IntPtr application_key;
        internal int application_key_size;
        internal IntPtr user_agent;
        internal IntPtr callbacks;
        internal IntPtr userdata;
        internal bool compress_playlists;
        internal bool dont_save_metadata_for_playlists;
        internal bool initially_unload_playlists;
        internal IntPtr device_id;
        internal IntPtr proxy;
        internal IntPtr proxy_username;
        internal IntPtr proxy_password;
        internal IntPtr ca_certs_filename;
        internal IntPtr tracefile;
    }

    internal struct sp_session_callbacks
    {
        internal IntPtr logged_in;
        internal IntPtr logged_out;
        internal IntPtr metadata_updated;
        internal IntPtr connection_error;
        internal IntPtr message_to_user;
        internal IntPtr notify_main_thread;
        internal IntPtr music_delivery;
        internal IntPtr play_token_lost;
        internal IntPtr log_message;
        internal IntPtr end_of_track;
        internal IntPtr streaming_error;
        internal IntPtr userinfo_updated;
        internal IntPtr start_playback;
        internal IntPtr stop_playback;
        internal IntPtr get_audio_buffer_stats;
        internal IntPtr offline_status_updated;
        internal IntPtr offline_error;
        internal IntPtr credentials_blob_updated;
        internal IntPtr connectionstate_updated;
        internal IntPtr scrobble_error;
        internal IntPtr private_session_mode_changed;
    }

    internal struct sp_audioformat
    {
        internal int sample_type;
        internal int sample_rate;
        internal int channels;
    }

    internal enum sp_error
    {
        OK = 0,
        BAD_API_VERSION = 1,
        API_INITIALIZATION_FAILED = 2,
        TRACK_NOT_PLAYABLE = 3,
        RESOURCE_NOT_LOADED = 4,
        APPLICATION_KEY = 5,
        BAD_USERNAME_OR_PASSWORD = 6,
        USER_BANNED = 7,
        UNABLE_TO_CONTACT_SERVER = 8,
        CLIENT_TOO_OLD = 9,
        OTHER_PERMANENT = 10,
        BAD_USER_AGENT = 11,
        MISSING_CALLBACK = 12,
        INVALID_INDATA = 13,
        INDEX_OUT_OF_RANGE = 14,
        USER_NEEDS_PREMIUM = 15,
        OTHER_TRANSIENT = 16,
        IS_LOADING = 17
    }

    internal enum sp_connectionstate
    {
        LOGGED_OUT = 0,
        LOGGED_IN = 1,
        DISCONNECTED = 2,
        UNDEFINED = 3
    }

    internal enum sp_bitrate
    {
        BITRATE_160k = 0,
        BITRATE_320k = 1
    }

    internal enum sp_radio_genre
    {
        ALT_POP_ROCK = 0x1,
        BLUES = 0x2,
        COUNTRY = 0x4,
        DISCO = 0x8,
        FUNK = 0x10,
        HARD_ROCK = 0x20,
        HEAVY_METAL = 0x40,
        RAP = 0x80,
        HOUSE = 0x100,
        JAZZ = 0x200,
        NEW_WAVE = 0x400,
        RNB = 0x800,
        POP = 0x1000,
        PUNK = 0x2000,
        REGGAE = 0x4000,
        POP_ROCK = 0x8000,
        SOUL = 0x10000,
        TECHNO = 0x20000
    }

    internal enum sp_albumtype
    {
        SP_ALBUMTYPE_ALBUM = 0,
        SP_ALBUMTYPE_SINGLE = 1,
        SP_ALBUMTYPE_COMPILATION = 2,
        SP_ALBUMTYPE_UNKNOWN = 3
    }

    internal enum sp_imageformat
    {
        SP_IMAGE_FORMAT_UNKNOWN = -1,
        SP_IMAGE_FORMAT_JPEG = 0
    }

    /// <summary>
    /// Playlist types.
    /// </summary>
    internal enum sp_playlist_type
    {
        /// <summary>
        /// A normal playlist.
        /// </summary>
        SP_PLAYLIST_TYPE_PLAYLIST = 0,
        /// <summary>
        /// Marks a folder starting point.
        /// </summary>
        SP_PLAYLIST_TYPE_START_FOLDER = 1,
        /// <summary>
        /// Marks a folder ending point.
        /// </summary>
        SP_PLAYLIST_TYPE_END_FOLDER = 2,
        /// <summary>
        /// Unknown entry.
        /// </summary>
        SP_PLAYLIST_TYPE_PLACEHOLDER = 3
    }
}
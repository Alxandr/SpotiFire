using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotiFire.SpotifyLib
{
    #region Delegates
    internal delegate void logged_in_delegate(IntPtr sessionPtr, Error error);
    internal delegate void logged_out_delegate(IntPtr sessionPtr);
    internal delegate void metadata_updated_delegate(IntPtr sessionPtr);
    internal delegate void connection_error_delegate(IntPtr sessionPtr, Error error);
    internal delegate void message_to_user_delegate(IntPtr sessionPtr, string message);
    internal delegate void notify_main_thread_delegate(IntPtr sessionPtr);
    internal delegate int music_delivery_delegate(IntPtr sessionPtr, IntPtr formatPtr, IntPtr framesPtr, int num_frames);
    internal delegate void play_token_lost_delegate(IntPtr sessionPtr);
    internal delegate void log_message_delegate(IntPtr sessionPtr, string data);
    internal delegate void end_of_track_delegate(IntPtr sessionPtr);
    internal delegate void streaming_error_delegate(IntPtr sessionPtr, Error error);
    internal delegate void userinfo_updated_delegate(IntPtr sessionPtr);
    internal delegate void start_playback_delegate(IntPtr sessionPtr);
    internal delegate void stop_playback_delegate(IntPtr sessionPtr);
    internal delegate void get_audio_buffer_stats_delegate(IntPtr sessionPtr, IntPtr statsPtr);
    internal delegate void offline_status_updated_delegate(IntPtr sessionPtr);
    internal delegate void offline_error_delegate(IntPtr sessionPtr, Error error);
    internal delegate void credentials_blob_updated_delegate(IntPtr sessionPtr, string blob);
    internal delegate void connectionstate_updated_delegate(IntPtr sessionPtr);
    internal delegate void scrobble_error_delegate(IntPtr sessionPtr, Error error);
    internal delegate void private_session_mode_changed_delegate(IntPtr sessionPtr, bool is_internal);
    #endregion

    #region Data Structures
    /// <summary>
    /// Audio format descriptor
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AudioFormat
    {
        SampleType _sampleType;
        int _sampleRate;
        int _channels;

        /// <summary>
        /// Sample type.
        /// </summary>
        public SampleType SampleType
        {
            get { return _sampleType; }
            set { _sampleType = value; }
        }

        /// <summary>
        /// Audio sample rate, in samples per second.
        /// </summary>
        public int SampleRate
        {
            get { return _sampleRate; }
            set { _sampleRate = value; }
        }

        /// <summary>
        /// Number of channels. Currently 1 or 2.
        /// </summary>
        public int Channels
        {
            get { return _channels; }
            set { _channels = value; }
        }
    }

    /// <summary>
    /// Buffer stats used by get_audio_buffer_stats callback.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AudioBufferStats
    {
        int _samples;
        int _stutter;

        /// <summary>
        /// Samples in buffer. 
        /// </summary>
        public int Samples
        {
            get { return _samples; }
            set { _samples = value; }
        }

        /// <summary>
        /// Number of stutters (audio dropouts) since last query.
        /// </summary>
        public int Stutter
        {
            get { return _stutter; }
            set { _stutter = value; }
        }
    }

    /// <summary>
    /// List of subscribers returned by <see cref="SpotiFire.SpotifyLib.IPlaylist.Subscribers"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SubscribersInfo
    {
        uint _count;
        IntPtr _subscribers;

        /// <summary>
        /// Number of elements in 'subscribers'.
        /// </summary>
        public uint Count
        {
            get { return _count; }
        }

        /// <summary>
        /// Array of canonical usernames.
        /// </summary>
        public string[] Subscribers
        {
            get
            {
                string[] ret = new string[_count];
                IntPtr current = _subscribers;
                for (var i = 0; i < _count; i++)
                {
                    ret[i] = libspotify.GetString(current);
                    current = IntPtr.Add(current, IntPtr.Size);
                }
                return ret;
            }
        }
    }

    /// <summary>
    /// Offline sync status
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct OfflineSyncStatus
    {
        int _queuedTracks;
        int _doneTracks;
        int _copiedTracks;
        int _willnotcopyTracks;
        int _errorTracks;
        bool _syncing;

        /// <summary>
        /// Queued tracks/bytes is things left to sync in current sync operation.
        /// </summary>
        public int QueuedTracks
        {
            get { return _queuedTracks; }
            set { _queuedTracks = value; }
        }

        /// <summary>
        /// Done tracks/bytes is things marked for sync that existed on device before current sync operation.
        /// </summary>
        public int DoneTracks
        {
            get { return _doneTracks; }
            set { _doneTracks = value; }
        }

        /// <summary>
        /// Copied tracks/bytes is things that has been copied in current sync operation.
        /// </summary>
        public int CopiedTracks
        {
            get { return _copiedTracks; }
            set { _copiedTracks = value; }
        }

        /// <summary>
        /// Tracks that are marked as synced but will not be copied (for various reasons).
        /// </summary>
        public int WillNotCopyTracks
        {
            get { return _willnotcopyTracks; }
            set { _willnotcopyTracks = value; }
        }

        /// <summary>
        /// A track is counted as error when something goes wrong while syncing the track.
        /// </summary>
        public int ErrorTracks
        {
            get { return _errorTracks; }
            set { _errorTracks = value; }
        }

        /// <summary>
        /// Set if sync operation is in progress.
        /// </summary>
        public bool Syncing
        {
            get { return _syncing; }
            set { _syncing = value; }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SessionCallbacks
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

    [StructLayout(LayoutKind.Sequential)]
    internal struct SessionConfig
    {
        internal int api_version;
        internal string cache_location;
        internal string settings_location;
        internal IntPtr application_key;
        internal int application_key_size;
        internal string user_agent;
        internal IntPtr callbacks;
        internal IntPtr userdata;
        internal bool compress_playlists;
        internal bool dont_save_metadata_for_playlists;
        internal bool initially_unload_playlists;
        internal string device_id;
        internal string proxy;
        internal string proxy_username;
        internal string proxy_password;
        internal string ca_certs_filename;
        internal string tracefile;
    }
    #endregion

    #region Enums
    /// <summary>
    /// Describes the current state of the connection
    /// </summary>
    public enum ConnectionState
    {
        /// <summary>
        /// User not yet logged in.
        /// </summary>
        LoggedOut = 0,

        /// <summary>
        /// Logged in against a Spotify access point.
        /// </summary>
        LoggedIn = 1,

        /// <summary>
        /// Was logged in, but has now been disconnected.
        /// </summary>
        Disconnected = 2,

        /// <summary>
        /// The connection state is undefined.
        /// </summary>
        Undefined = 3,

        /// <summary>
        /// Logged in in offline mode.
        /// </summary>
        Offline = 4
    }

    /// <summary>
    /// Sample type descriptor.
    /// </summary>
    public enum SampleType
    {
        /// <summary>
        /// 16-bit signed integer samples.
        /// </summary>
        Int16NativeEndian = 0
    }

    /// <summary>
    /// Bitrate definitions for music streaming.
    /// </summary>
    public enum BitRate
    {
        /// <summary>
        /// Bitrate 160kbps.
        /// </summary>
        Bitrate160k = 0,

        /// <summary>
        /// Bitrate 320kbps.
        /// </summary>
        Bitrate320k = 1,

        /// <summary>
        /// Bitrate 96kbps.
        /// </summary>
        Bitrate96k = 2
    }

    /// <summary>
    /// Playlist types.
    /// </summary>
    public enum PlaylistType
    {
        /// <summary>
        /// A normal playlist.
        /// </summary>
        Playlist = 0,

        /// <summary>
        /// Marks a folder starting point.
        /// </summary>
        StartFolder = 1,

        /// <summary>
        /// Marks a folder ending point.
        /// </summary>
        EndFolder = 2,

        /// <summary>
        /// Unknown entry.
        /// </summary>
        Placeholder = 3
    }

    /// <summary>
    /// Search types.
    /// </summary>
    public enum SearchType
    {
        Standard = 0,
        Suggest = 1,
    }

    /// <summary>
    /// Playlist offline status.
    /// </summary>
    public enum PlaylistOfflineStatus
    {
        /// <summary>
        /// Playlist is not offline enabled.
        /// </summary>
        No = 0,

        /// <summary>
        /// Playlist is synchronized to local storage.
        /// </summary>
        Yes = 1,

        /// <summary>
        /// This playlist is currently downloading. Only one playlist can be in this state any given time.
        /// </summary>
        Downloading = 2,

        /// <summary>
        /// Playlist is queued for download.
        /// </summary>
        Waiting = 3
    }

    /// <summary>
    /// Track availability
    /// </summary>
    public enum TrackAvailability
    {
        /// <summary>
        /// Track is not available.
        /// </summary>
        Unavailable = 0,

        /// <summary>
        /// Track is available and can be played.
        /// </summary>
        Available = 1,

        /// <summary>
        /// Track can not be streamed using this account.
        /// </summary>
        NotStreamable = 2,

        /// <summary>
        /// Track not available on artist's reqeust.
        /// </summary>
        BannedByArtist = 3
    }

    /// <summary>
    /// Track offline status.
    /// </summary>
    public enum TrackOfflineStatus
    {
        /// <summary>
        /// Not marked for offline.
        /// </summary>
        No = 0,

        /// <summary>
        /// Waiting for download.
        /// </summary>
        Waiting = 1,

        /// <summary>
        /// Currently downloading.
        /// </summary>
        Downloading = 2,

        /// <summary>
        /// Downloaded OK and can be played.
        /// </summary>
        Done = 3,

        /// <summary>
        /// Error during download.
        /// </summary>
        Error = 4,

        /// <summary>
        /// Downloaded OK but not playable due to expiery.
        /// </summary>
        DoneExpired = 5,

        /// <summary>
        /// Waiting because device have reached max number of allowed tracks.
        /// </summary>
        LimitExceeded = 6,

        /// <summary>
        /// Downloaded OK and available but scheduled for re-download.
        /// </summary>
        DoneResync = 7
    }

    /// <summary>
    /// Image size.
    /// </summary>
    public enum ImageSize
    {
        /// <summary>
        /// Normal image size.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Small image size.
        /// </summary>
        Small = 1,

        /// <summary>
        /// Large image size.
        /// </summary>
        Large = 2
    }

    /// <summary>
    /// Current connection type set using <see cref="SpotiFire.SpotifyLib.ISession.ConnectionType"/>.
    /// </summary>
    public enum ConnectionType
    {
        /// <summary>
        /// Connection type unknown (Default).
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// No connection.
        /// </summary>
        None = 1,

        /// <summary>
        /// Mobile data (EDGE, 3G, etc).
        /// </summary>
        Mobile = 2,

        /// <summary>
        /// Roamed mobile data (EDGE, 3G, etc).
        /// </summary>
        MobileRoaming = 3,

        /// <summary>
        /// Wireless connection.
        /// </summary>
        Wifi = 4,

        /// <summary>
        /// Ethernet cable, etc.
        /// </summary>
        Wired = 5
    }

    /// <summary>
    /// Connection rules, bitwise OR of flags.
    /// 
    /// The default is <see cref="SpotiFire.SpotifyLib.ConnectionType.Network"/> | <see cref="SpotiFire.SpotifyLib.ConnectionType.AllowSync"/>
    /// </summary>
    [Flags]
    public enum ConnectionRules
    {
        /// <summary>
        /// Allow network traffic. When not set libspotify will force itself into offline mode.
        /// </summary>
        Network = 0x1,

        /// <summary>
        /// Allow network traffic even if roaming.
        /// </summary>
        NetworkIfRoaming = 0x2,

        /// <summary>
        /// Set to allow syncing of offline content over mobile connections.
        /// </summary>
        AllowSyncOverMobile = 0x4,

        /// <summary>
        /// Set to allow syncing of offline content over WiFi.
        /// </summary>
        AllowSyncOverWifi = 0x8
    }

    public enum ArtistBrowseType
    {
        /// <summary>
        /// All information except tophit tracks This mode is deprecated and will removed in a future release
        /// </summary>
        Full = 0,

        /// <summary>
        /// Only albums and data about them, no tracks.
        /// </summary>
        NoTracks = 1,

        /// <summary>
        /// Only return data about the artist (artist name, similar artist biography, etc No tracks or album will be abailable.
        /// </summary>
        NoAlbums = 2
    }

    /// <summary>
    /// Social provider used for scrobbing
    /// </summary>
    public enum SocialProvider
    {
        Spotify = 0,
        Facebook = 1,
        LastFM = 2
    }

    /// <summary>
    /// Scrobbling state
    /// </summary>
    public enum ScrobblingState
    {
        UseGlobalSettings = 0,
        LocalEnabled = 1,
        LocalDisabled = 2,
        GlobalEnabled = 3,
        GlobalDisabled = 4
    }
    #endregion

    static partial class libspotify
    {
        #region Defines
        internal const int SPOTIFY_API_VERSION = 12;
        internal static readonly object Mutex = new object();
        #endregion

        /// <summary>
        /// Initialize a session. The session returned will be initialized,
        /// but you will need to log in before you can perform any other operation.
        /// Currently it is not supported to have multiple active sessions,
        /// and it's recommended to only call this once per process.
        /// </summary>
        /// <param name="configPtr">The configuration to use for the session.</param>
        /// <param name="sessionPtrPtr">If successful, a new session - otherwise IntPtr.Null.</param>
        /// <returns>One of the following errors, from Error Error_OK Error_BAD_API_VERSION
        /// Error_BAD_USER_AGENT Error_BAD_APPLICATION_KEY Error_API_INITIALIZATION_FAILED Error_INVALID_DEVICE_ID</returns>
        [DllImport("libspotify")]
        static internal extern Error sp_session_create(ref SessionConfig configPtr, out IntPtr sessionPtr);

        /// <summary>
        /// Release the session. This will clean up all data and connections associated with the session.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from sp_session_create()</param>
        /// <returns>One of the following errors, from Error Error_OK</returns>
        [DllImport("libspotify")]
        static internal extern Error sp_session_release(IntPtr sessionPtr);

        /// <summary>
        /// Logs in the specified username/password combo. This initiates the login in the background. A callback is called when login is complete.
        /// An application MUST NEVER store the user's password in clear text. If automatic relogin is required, use sp_session_relogin().
        /// </summary>
        /// <param name="sessionPtr">Your session object.</param>
        /// <param name="username">The username to log in.</param>
        /// <param name="password">The password for the specified username.</param>
        /// <param name="rememberMe">If set, the username / password will be remembered by libspotify.</param>
        /// <param name="blob">If you have received a blob in the credentials_blob_updated you can pas this here instead of password.</param>
        /// <returns>One of the following errors, from Error Error_OK.</returns>
        [DllImport("libspotify")]
        static internal extern Error sp_session_login(IntPtr sessionPtr, string username, string password, bool rememberMe, string blob);

        /// <summary>
        /// Log in the remembered user if last user that logged in logged in with remember_me set. If no credentials are stored, this will return Error_NO_CREDENTIALS.
        /// </summary>
        /// <param name="sessionPtr">Your session object.</param>
        /// <returns>One of the following errors, from Error Error_OK Error_NO_CREDENTIALS.</returns>
        [DllImport("libspotify")]
        static internal extern Error sp_session_relogin(IntPtr sessionPtr);

        /// <summary>
        /// Get username of the user that will be logged in via sp_session_relogin().
        /// </summary>
        /// <param name="sessionPtr">Your session object.</param>
        /// <param name="charBufferPtr">The buffer to hold the username.</param>
        /// <param name="bufferSize">The max size of the buffer that will hold the username. The resulting string is guaranteed to always be null terminated if buffer_size > 0.</param>
        /// <returns>The number of characters in the username. If value is greater or equal than buffer_size, output was truncated.
        /// If returned value is -1 no credentials are stored in libspotify.</returns>
        [DllImport("libspotify")]
        static internal extern int sp_session_remembered_user(IntPtr sessionPtr, IntPtr charBufferPtr, int bufferSize);

        /// <summary>
        /// Get a pointer to a string representing the user's login username.
        /// </summary>
        /// <param name="sessionPtr">Your session object.</param>
        /// <returns>A string representing the login username.</returns>
        [DllImport("libspotify")]
        static internal extern string sp_session_user_name(IntPtr sessionPtr);

        /// <summary>
        /// Remove stored credentials in libspotify. If no credentials are currently stored, nothing will happen.
        /// </summary>
        /// <param name="sessionPtr">Your session object.</param>
        /// <returns>One of the following errors, from Error Error_OK.</returns>
        [DllImport("libspotify")]
        static internal extern Error sp_session_forget_me(IntPtr sessionPtr);

        /// <summary>
        /// Fetches the currently logged in user.
        /// </summary>
        /// <param name="sessionPtr">Your session object.</param>
        /// <returns>The logged in user (or NULL if not logged in).</returns>
        [DllImport("libspotify")]
        static internal extern IntPtr sp_session_user(IntPtr sessionPtr);

        /// <summary>
        /// Logs out the currently logged in user.
        /// 
        /// Always call this before terminating the application and libspotify is currently logged in. Otherwise, the settings and cache may be lost.
        /// </summary>
        /// <param name="sessionPtr">Your session object.</param>
        /// <returns>One of the following errors, from Error Error_OK.</returns>
        [DllImport("libspotify")]
        static internal extern Error sp_session_logout(IntPtr sessionPtr);

        /// <summary>
        /// Flush the caches.
        /// 
        /// This will make libspotify write all data that is meant to be stored on disk to the disk immediately.
        /// libspotify does this periodically by itself and also on logout. So under normal conditions this should never need to be used.
        /// </summary>
        /// <param name="sessionPtr">Your session object.</param>
        /// <returns>One of the following errors, from Error Error_OK.</returns>
        [DllImport("libspotify")]
        static internal extern Error sp_session_flush_caches(IntPtr sessionPtr);

        /// <summary>
        /// The connection state of the specified session.
        /// </summary>
        /// <param name="sessionPtr">Your session object.</param>
        /// <returns>The connection state - see the <see cref="SpotiFire.SpotifyLib.ConnectionState"/> enum for possible values</returns>
        [DllImport("libspotify")]
        static internal extern ConnectionState sp_session_connectionstate(IntPtr sessionPtr);

        /// <summary>
        /// The userdata associated with the session.
        /// </summary>
        /// <param name="sessionPtr">Your session object.</param>
        /// <returns>The userdata that was passed in on session creation.</returns>
        [DllImport("libspotify")]
        static internal extern IntPtr sp_session_userdata(IntPtr sessionPtr);

        /// <summary>
        /// Set maximum cache size.
        /// </summary>
        /// <param name="sessionPtr">Your session object.</param>
        /// <param name="size">Maximum cache size in megabytes. Setting it to 0 (the default) will let libspotify automatically resize the cache (10% of disk free space).</param>
        /// <returns>One of the following errors, from Error Error_OK.</returns>
        [DllImport("libspotify")]
        static internal extern Error sp_session_set_cache_size(IntPtr sessionPtr, int size);

        /// <summary>
        /// Make the specified session process any pending events.
        /// </summary>
        /// <param name="sessionPtr">Your session object.</param>
        /// <param name="nextTimeout">Stores the time (in milliseconds) until you should call this function again.</param>
        /// <returns>One of the following errors, from Error Error_OK.</returns>
        [DllImport("libspotify")]
        static internal extern Error sp_session_process_events(IntPtr sessionPtr, out int nextTimeout);

        /// <summary>
        /// Loads the specified track.
        /// 
        /// After successfully loading the track, you have the option of running sp_session_player_play() directly,
        /// or using sp_session_player_seek() first.
        /// When this call returns, the track will have been loaded, unless an error occurred.
        /// </summary>
        /// <param name="sessionPtr">Your session object.</param>
        /// <param name="trackPtr">The track to be loaded.</param>
        /// <returns>One of the following errors, from Error Error_OK Error_MISSING_CALLBACK Error_TRACK_NOT_PLAYABLE.</returns>
        [DllImport("libspotify")]
        static internal extern Error sp_session_player_load(IntPtr sessionPtr, IntPtr trackPtr);

        /// <summary>
        /// Seek to position in the currently loaded track.
        /// </summary>
        /// <param name="sessionPtr">Your session object.</param>
        /// <param name="offset">Track position, in milliseconds.</param>
        /// <returns>One of the following errors, from Error Error_OK.</returns>
        [DllImport("libspotify")]
        static internal extern Error sp_session_player_seek(IntPtr sessionPtr, int offset);

        /// <summary>
        /// Play or pause the currently loaded track.
        /// </summary>
        /// <param name="sessionPtr">Your session object.</param>
        /// <param name="play">If set to true, playback will occur. If set to false, the playback will be paused.</param>
        /// <returns>One of the following errors, from Error Error_OK.</returns>
        [DllImport("libspotify")]
        static internal extern Error sp_session_player_play(IntPtr sessionPtr, bool play);

        /// <summary>
        /// Stops the currently playing track.
        /// 
        /// This frees some resources held by libspotify to identify the currently playing track.
        /// </summary>
        /// <param name="sessionPtr">Your session object.</param>
        /// <returns>One of the following errors, from Error Error_OK.</returns>
        [DllImport("libspotify")]
        static internal extern Error sp_session_player_unload(IntPtr sessionPtr);

        /// <summary>
        /// Prefetch a track.
        /// 
        /// Instruct libspotify to start loading of a track into its cache. This could be done by an application just before the current track ends.
        /// </summary>
        /// <param name="sessionPtr">Your session object.</param>
        /// <param name="trackPtr">The track to be prefetched.</param>
        /// <returns>One of the following errors, from Error Error_NO_CACHE Error_OK.</returns>
        /// <remarks>Prefetching is only possible if a cache is configured.</remarks>
        [DllImport("libspotify")]
        static internal extern Error sp_session_player_prefetch(IntPtr sessionPtr, IntPtr trackPtr);

        /// <summary>
        /// Returns the playlist container for the currently logged in user.
        /// </summary>
        /// <param name="sessionPtr">Your session object.</param>
        /// <returns>Playlist container object, NULL if not logged in.</returns>
        [DllImport("libspotify")]
        static internal extern IntPtr sp_session_playlistcontainer(IntPtr sessionPtr);

        /// <summary>
        /// Returns an inbox playlist for the currently logged in user.
        /// </summary>
        /// <param name="sessionPtr">Session object.</param>
        /// <returns>A playlist or NULL if no user is logged in.</returns>
        [DllImport("libspotify")]
        static internal extern IntPtr sp_session_inbox_create(IntPtr sessionPtr);

        /// <summary>
        /// Returns the starred list for the current user.
        /// </summary>
        /// <param name="sessionPtr">Session object.</param>
        /// <returns>A playlist or NULL if no user is logged in.</returns>
        [DllImport("libspotify")]
        static internal extern IntPtr sp_session_starred_create(IntPtr sessionPtr);

        /// <summary>
        /// Returns the starred list for a user.
        /// </summary>
        /// <param name="sessionPtr">Session object.</param>
        /// <param name="canonicalUsername">Canonical username.</param>
        /// <returns>A playlist or NULL if no user is logged in.</returns>
        [DllImport("libspotify")]
        static internal extern IntPtr sp_session_starred_for_user_create(IntPtr sessionPtr, string canonicalUsername);

        /// <summary>
        /// Return the published container for a given <paramref name="canonicalUsername"/>, or the currently logged in user if canonical_username is null.
        /// </summary>
        /// <param name="sessionPtr">Your session object.</param>
        /// <param name="canonicalUsername">The canonical username, or null.</param>
        /// <returns>Playlist container object, NULL if not logged in.</returns>
        [DllImport("libspotify")]
        static internal extern IntPtr sp_session_publishedcontainer_for_user_create(IntPtr sessionPtr, string canonicalUsername);

        /// <summary>
        /// Set preferred bitrate for music streaming.
        /// </summary>
        /// <param name="sessionPtr">Session object.</param>
        /// <param name="bitrate">Preferred bitrate, see sp_bitrate for possible values.</param>
        /// <returns>One of the following errors, from Error Error_OK Error_INVALID_ARGUMENT.</returns>
        [DllImport("libspotify")]
        static internal extern Error sp_session_preferred_bitrate(IntPtr sessionPtr, BitRate bitrate);

        /// <summary>
        /// Set preferred bitrate for offline sync.
        /// </summary>
        /// <param name="sessionPtr">Session object.</param>
        /// <param name="bitrate">Preferred bitrate, see sp_bitrate for possible values.</param>
        /// <param name="allowResync">Set to true if libspotify should resynchronize already synchronized tracks. Usually you should set this to false.</param>
        /// <returns>One of the following errors, from Error Error_OK Error_INVALID_ARGUMENT.</returns>
        [DllImport("libspotify")]
        static internal extern Error sp_session_preferred_offline_bitrate(IntPtr sessionPtr, BitRate bitrate, bool allowResync);

        /// <summary>
        /// Return status of volume normalization.
        /// </summary>
        /// <param name="sessionPtr">Session object.</param>
        /// <returns>true iff volume normalization is enabled.</returns>
        [DllImport("libspotify")]
        static internal extern bool sp_session_get_volume_normalization(IntPtr sessionPtr);

        /// <summary>
        /// Set volume normalization.
        /// </summary>
        /// <param name="sessionPtr">Session object.</param>
        /// <param name="on">True iff volume normalization should be enabled.</param>
        /// <returns>One of the following errors, from Error Error_OK.</returns>
        [DllImport("libspotify")]
        static internal extern Error sp_session_set_volume_normalization(IntPtr sessionPtr, bool on);

        /// <summary>
        /// Set if private session is enabled. This disables sharing what the user is
        /// listening to to services such as Spotify Social, Facebook and LastFM.
        /// The private session will last for a time, and then libspotify will
        /// revert to the normal state. The private session is prolonged by user activity.
        /// </summary>
        /// <param name="sessionPtr">Session object.</param>
        /// <param name="enabled">True iff private session should be enabled.</param>
        /// <returns>One of the following errors, from Error Error_OK</returns>
        [DllImport("libspotify")]
        static internal extern Error sp_session_set_private_session(IntPtr sessionPtr, bool enabled);

        /// <summary>
        /// Return True if private session is enabled.
        /// </summary>
        /// <param name="sessionPtr">Session object.</param>
        /// <returns>True if private session is enabled.</returns>
        [DllImport("libspotify")]
        static internal extern bool sp_session_is_private_session(IntPtr sessionPtr);

        /// <summary>
        /// Set if scrobbling is enabled. This api allows setting local overrides of the global scrobbling settings.
        /// Changing the global settings are currently not supported.
        /// </summary>
        /// <param name="sessionPtr">Session object.</param>
        /// <param name="provider">The scrobbling provider referred to.</param>
        /// <param name="state">The state to set the provider to.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        static internal extern Error sp_session_set_scrobbling(IntPtr sessionPtr, SocialProvider provider, ScrobblingState state);

        /// <summary>
        /// Return the scrobbling state. This makes it possible to find out if scrobbling is locally overrided or if the global setting is used.
        /// </summary>
        /// <param name="sessionPtr">Session object.</param>
        /// <param name="provider">The scrobbling provider referred to.</param>
        /// <param name="state">The output variable receiving the sp_scrobbling_state state.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        static internal extern Error sp_session_is_scrobbling(IntPtr sessionPtr, SocialProvider provider, out ScrobblingState state);

        /// <summary>
        /// Return True if scrobbling settings should be shown to the user. Currently this setting is relevant only to the facebook provider.
        /// The returned value may be false if the user is not connected to facebook, or if the user has opted out from facebook social graph.
        /// </summary>
        /// <param name="sessionPtr">Session object.</param>
        /// <param name="provider">The scrobbling provider referred to.</param>
        /// <param name="isPossible">True iff scrobbling is possible.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        static internal extern Error sp_session_is_scrobbling_possible(IntPtr sessionPtr, SocialProvider provider, out bool isPossible);

        /// <summary>
        /// Set the user's credentials with a social provider. Currently this is only relevant for LastFm.
        /// Call sp_session_set_scrobbling to force an authentication attempt with the LastFm server. If authentication fails a scrobble_error callback will be sent.
        /// </summary>
        /// <param name="sessionPtr">Session object.</param>
        /// <param name="provider">The scrobbling provider referred to.</param>
        /// <param name="username">The user name.</param>
        /// <param name="password">The password.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        static internal extern Error sp_session_set_social_credentials(IntPtr sessionPtr, SocialProvider provider, string username, string password);

        /// <summary>
        /// Set to true if the connection is currently routed over a roamed connectivity.
        /// </summary>
        /// <param name="sessionPtr">Session object.</param>
        /// <param name="type">Connection type.</param>
        /// <returns>Used in conjunction with sp_session_set_connection_rules() to control how libspotify should behave in respect to network activity and offline synchronization.</returns>
        [DllImport("libspotify")]
        static internal extern Error sp_session_set_connection_type(IntPtr sessionPtr, ConnectionType type);

        /// <summary>
        /// Set rules for how libspotify connects to Spotify servers and synchronizes offline content.
        /// </summary>
        /// <param name="sessionPtr">Session object.</param>
        /// <param name="rules">Connection rules.</param>
        /// <returns>Used in conjunction with sp_session_set_connection_type() to control how libspotify should behave in respect to network activity and offline synchronization.</returns>
        [DllImport("libspotify")]
        static internal extern Error sp_session_set_connection_rules(IntPtr sessionPtr, ConnectionRules rules);

        /// <summary>
        /// Get total number of tracks that needs download before everything from all playlists that is marked for offline is fully synchronized.
        /// </summary>
        /// <param name="sessionPtr">Session object.</param>
        /// <returns>Number of tracks.</returns>
        [DllImport("libspotify")]
        static internal extern int sp_offline_tracks_to_sync(IntPtr sessionPtr);

        /// <summary>
        /// Return number of playlisys that is marked for offline synchronization.
        /// </summary>
        /// <param name="sessionPtr">Session object.</param>
        /// <returns>Number of playlists.</returns>
        [DllImport("libspotify")]
        static internal extern int sp_offline_num_playlists(IntPtr sessionPtr);

        /// <summary>
        /// Return offline synchronization status. When the internal status is updated the offline_status_updated() callback will be invoked.
        /// </summary>
        /// <param name="sessionPtr">Session object.</param>
        /// <param name="status">Status object that will be filled with info.</param>
        /// <returns>False if no synching is in progress (in which case the contents of status is undefined).</returns>
        [DllImport("libspotify")]
        static internal extern bool sp_offline_sync_get_status(IntPtr sessionPtr, out OfflineSyncStatus status);

        /// <summary>
        /// Return remaining time (in seconds) until the offline key store expires and the user is required to relogin.
        /// </summary>
        /// <param name="sessionPtr">Session object.</param>
        /// <returns>Seconds until expiration.</returns>
        [DllImport("libspotify")]
        static internal extern int sp_offline_time_left(IntPtr sessionPtr);

        /// <summary>
        /// Get currently logged in users country updated the offline_status_updated() callback will be invoked.
        /// </summary>
        /// <param name="sessionPtr">Session object.</param>
        /// <returns>Country encoded in an integer 'SE' = 'S' &lt;&lt; 8 | 'E'</returns>
        [DllImport("libspotify")]
        static internal extern int sp_session_user_country(IntPtr sessionPtr);
    }
}

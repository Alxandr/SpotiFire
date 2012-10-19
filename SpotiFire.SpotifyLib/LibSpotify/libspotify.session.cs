using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotiFire
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
    /// List of subscribers returned by <see cref="SpotiFire.Types.IPlaylist.Subscribers"/>
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

        ///// <summary>
        ///// Array of canonical usernames.
        ///// </summary>
        //public string[] Subscribers
        //{
        //    get
        //    {
        //        string[] ret = new string[_count];
        //        IntPtr current = _subscribers;
        //        for (var i = 0; i < _count; i++)
        //        {
        //            ret[i] = libspotify.GetString(current);
        //            current = IntPtr.Add(current, IntPtr.Size);
        //        }
        //        return ret;
        //    }
        //}
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
    /// Current connection type set using <see cref="SpotiFire.Types.ISession.ConnectionType"/>.
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
    /// The default is <see cref="SpotiFire.Types.ConnectionType.Network"/> | <see cref="SpotiFire.Types.ConnectionType.AllowSync"/>
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

    static class libspotify
    {
        static internal readonly object Mutex = new object();

        internal static string ImageIdToString(IntPtr idPtr)
        {
            if (idPtr == IntPtr.Zero)
                return string.Empty;

            byte[] id = new byte[20];
            Marshal.Copy(idPtr, id, 0, 20);

            return ImageIdToString(id);
        }

        private static string ImageIdToString(byte[] id)
        {
            if (id == null)
                return string.Empty;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (byte b in id)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }

        internal static byte[] StringToImageId(string id)
        {
            if (string.IsNullOrEmpty(id) || id.Length != 40)
                return null;
            byte[] ret = new byte[20];
            try
            {
                for (int i = 0; i < 20; i++)
                {
                    ret[i] = byte.Parse(id.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
                }
                return ret;
            }
            catch
            {
                return null;
            }
        }
    }
}

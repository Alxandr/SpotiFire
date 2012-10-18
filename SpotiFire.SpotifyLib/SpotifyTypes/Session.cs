using NLog;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using SPSession = SpotiFire.Session;

namespace SpotiFire.SpotifyLib
{
    public delegate void SessionEventHandler(ISession sender, SessionEventArgs e);
    public delegate void MusicDeliveryEventHandler(ISession sender, MusicDeliveryEventArgs e);


    internal class Session : ISession
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        #region Static declarations
        private static Dictionary<IntPtr, Session> sessions = new Dictionary<IntPtr, Session>();
        private static SessionCallbacks callbacks;
        #endregion

        #region Declarations
        internal IntPtr sessionPtr = IntPtr.Zero;
        internal bool shutdown = false;

        private Thread mainThread;
        private AutoResetEvent mainThreadNotification = new AutoResetEvent(false);

        private Queue<EventWorkItem> eventQueue = new Queue<EventWorkItem>();

        private bool disposed = false;

        private IPlaylistContainer playlistContainer = null;

        #endregion

        #region Spotify Event Handlers
        private static logged_in_delegate logged_in = new logged_in_delegate(LoggedInCallback);
        private static logged_out_delegate logged_out = new logged_out_delegate(LoggedOutCallback);
        private static metadata_updated_delegate metadata_updated = new metadata_updated_delegate(MetadataUpdatedCallback);
        private static connection_error_delegate connection_error = new connection_error_delegate(ConnectionErrorCallback);
        private static message_to_user_delegate message_to_user = new message_to_user_delegate(MessageToUserCallback);
        private static notify_main_thread_delegate notify_main_thread = new notify_main_thread_delegate(NotifyMainThreadCallback);
        private static music_delivery_delegate music_delivery = new music_delivery_delegate(MusicDeliveryCallback);
        private static play_token_lost_delegate play_token_lost = new play_token_lost_delegate(PlayTokenLostCallback);
        private static log_message_delegate log_message = new log_message_delegate(LogMessageCallback);
        private static end_of_track_delegate end_of_track = new end_of_track_delegate(EndOfTrackCallback);
        private static streaming_error_delegate streaming_error = new streaming_error_delegate(StreamingErrorCallback);
        private static userinfo_updated_delegate userinfo_updated = new userinfo_updated_delegate(UserinfoUpdatedCallback);
        private static start_playback_delegate start_playback = new start_playback_delegate(StartPlaybackCallback);
        private static stop_playback_delegate stop_playback = new stop_playback_delegate(StopPlaybackCallback);
        private static get_audio_buffer_stats_delegate get_audio_buffer_stats = new get_audio_buffer_stats_delegate(GetAudioBufferStatsCallback);
        private static offline_status_updated_delegate offline_status_updated = new offline_status_updated_delegate(OfflineStatusUpdatedCallback);
        private static offline_error_delegate offline_error = new offline_error_delegate(OfflineErrorCallback);
        private static credentials_blob_updated_delegate credentials_blob_updated = new credentials_blob_updated_delegate(CredentialsBlobUpdatedCallback);
        private static connectionstate_updated_delegate connectionstate_updated = new connectionstate_updated_delegate(ConnectionstateUpdatedCallback);
        private static scrobble_error_delegate scrobble_error = new scrobble_error_delegate(ScrobbleErrorCallback);
        private static private_session_mode_changed_delegate private_session_mode_changed = new private_session_mode_changed_delegate(PrivateSessionModeChangedCallback);
        #endregion

        #region Properties
        public IPlaylistContainer PlaylistContainer
        {
            get
            {
                return playlistContainer;
            }
        }

        internal bool ProcExit
        {
            get;
            private set;
        }
        #endregion

        #region Constructor
        private Session(byte[] applicationKey, string cacheLocation, string settingsLocation, string userAgent)
        {
            ProcExit = false;
            SessionConfig config = new SessionConfig();

            config.api_version = SpotiFire.Session.__ApiVersion();
            config.cache_location = cacheLocation;
            config.settings_location = settingsLocation;
            config.user_agent = userAgent;
            config.compress_playlists = false;
            config.dont_save_metadata_for_playlists = false;
            config.initially_unload_playlists = false;

            int size = Marshal.SizeOf(callbacks);
            config.callbacks = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(callbacks, config.callbacks, true);

            size = Marshal.SizeOf(config);
            IntPtr configPtr = Marshal.AllocHGlobal(size);

            config.application_key = IntPtr.Zero;

            try
            {
                config.application_key = Marshal.AllocHGlobal(applicationKey.Length);
                Marshal.Copy(applicationKey, 0, config.application_key, applicationKey.Length);

                lock (libspotify.Mutex)
                    config.application_key_size = applicationKey.Length;

                logger.Debug("Creating session");
                sessionPtr = IntPtr.Zero;
                Marshal.StructureToPtr(config, configPtr, false);
                Error res = (Error)SPSession.create(configPtr, out sessionPtr);

                if (res != Error.OK)
                    throw new SpotifyException(res);

                mainThread = new Thread(new ThreadStart(MainThread));
                mainThread.IsBackground = true;
                mainThread.Start();
            }
            finally
            {
                if (config.application_key != IntPtr.Zero)
                    Marshal.FreeHGlobal(config.application_key);
                //if (configPtr != IntPtr.Zero)
                //    Marshal.FreeHGlobal(configPtr);
            }

            AppDomain.CurrentDomain.DomainUnload += new EventHandler(CurrentDomain_OnExit);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_OnExit);
        }

        void CurrentDomain_OnExit(object sender, EventArgs e)
        {
            ProcExit = true;
            this.Dispose();
            (sender as AppDomain).DomainUnload -= new EventHandler(CurrentDomain_OnExit);
            (sender as AppDomain).ProcessExit -= new EventHandler(CurrentDomain_OnExit);
        }

        static Session()
        {
            lock (libspotify.Mutex)
            {
                callbacks = new SessionCallbacks();
                callbacks.logged_in = Marshal.GetFunctionPointerForDelegate(logged_in);
                callbacks.logged_out = Marshal.GetFunctionPointerForDelegate(logged_out);
                callbacks.metadata_updated = Marshal.GetFunctionPointerForDelegate(metadata_updated);
                callbacks.connection_error = Marshal.GetFunctionPointerForDelegate(connection_error);
                callbacks.message_to_user = Marshal.GetFunctionPointerForDelegate(message_to_user);
                callbacks.notify_main_thread = Marshal.GetFunctionPointerForDelegate(notify_main_thread);
                callbacks.music_delivery = Marshal.GetFunctionPointerForDelegate(music_delivery);
                callbacks.play_token_lost = Marshal.GetFunctionPointerForDelegate(play_token_lost);
                callbacks.log_message = Marshal.GetFunctionPointerForDelegate(log_message);
                callbacks.end_of_track = Marshal.GetFunctionPointerForDelegate(end_of_track);
                callbacks.streaming_error = Marshal.GetFunctionPointerForDelegate(streaming_error);
                callbacks.userinfo_updated = Marshal.GetFunctionPointerForDelegate(userinfo_updated);
                callbacks.start_playback = Marshal.GetFunctionPointerForDelegate(start_playback);
                callbacks.stop_playback = Marshal.GetFunctionPointerForDelegate(stop_playback);
                callbacks.get_audio_buffer_stats = Marshal.GetFunctionPointerForDelegate(get_audio_buffer_stats);
                callbacks.offline_status_updated = Marshal.GetFunctionPointerForDelegate(offline_status_updated);
                callbacks.offline_error = Marshal.GetFunctionPointerForDelegate(offline_error);
                callbacks.credentials_blob_updated = Marshal.GetFunctionPointerForDelegate(credentials_blob_updated);
                callbacks.connectionstate_updated = Marshal.GetFunctionPointerForDelegate(connectionstate_updated);
                callbacks.scrobble_error = Marshal.GetFunctionPointerForDelegate(scrobble_error);
                callbacks.private_session_mode_changed = Marshal.GetFunctionPointerForDelegate(private_session_mode_changed);
            }
        }

        internal static Task<Session> Create(byte[] applicationKey, string cacheLocation, string settingsLocation, string userAgent)
        {
            return Task.Run(() =>
            {
                lock (libspotify.Mutex)
                {
                    if (sessions.Count > 0)
                        throw new InvalidOperationException("libspotify can only handle one session at the moment");

                    Session instance = new Session(applicationKey, cacheLocation, settingsLocation, userAgent);
                    sessions.Add(instance.sessionPtr, instance);
                    return instance;
                }
            });
        }
        #endregion

        #region Destructor
        ~Session()
        {
            this.Dispose();
        }
        #endregion

        #region Threads
        private void MainThread()
        {
            logger.Debug("Starting main thread");
            int waitTime = 0;
            while (!shutdown)
            {
                mainThreadNotification.WaitOne(waitTime, false);
                do
                {
                    lock (libspotify.Mutex)
                    {
                        try
                        {
                            if (this.disposed)
                                throw new Exception();
                            SPSession.process_events(sessionPtr, out waitTime);
                        }
                        catch
                        {
                            waitTime = 1000;
                        }
                    }
                }
                while (waitTime == 0);
            }
            logger.Debug("Stopping main thread");
        }
        #endregion

        #region HelperMethods
        private static Session GetSession(IntPtr sessionPtr)
        {
            Session s;
            if (sessions.TryGetValue(sessionPtr, out s))
                return s;
            else
                return null;
        }

        private static Delegate CreateDelegate<T>(Func<Session, Action<T>> expr, Session s) where T : EventArgs
        {
            return expr(s);
        }

        internal void EnqueueEventWorkItem(EventWorkItem eventWorkerItem)
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                try
                {
                    eventWorkerItem.Execute();
                }
                catch (Exception ex)
                {
                    OnException(new SessionEventArgs(ex.ToString()));
                }
            });
        }
        #endregion

        #region Spotify Event Handler Functions
        private static void LoggedInCallback(IntPtr sessionPtr, Error error)
        {
            logger.Trace("LoggedInCallback: {0}", error);
            Session s = GetSession(sessionPtr);
            if (s == null)
                return;

            if ((s.ConnectionState == ConnectionState.LoggedIn || s.ConnectionState == ConnectionState.Offline) && error == Error.OK)
                s.playlistContainer = SpotifyLib.PlaylistContainer.Get(s, SPSession.playlistcontainer(sessionPtr));

            s.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<SessionEventArgs>(se => se.OnLoginComplete, s), new SessionEventArgs(error)));
        }

        private static void LoggedOutCallback(IntPtr sessionPtr)
        {
            logger.Trace("LoggedOutCallback");
            Session s = GetSession(sessionPtr);
            if (s == null)
                return;

            if (s.PlaylistContainer != null)
            {
                s.playlistContainer.Dispose();
                s.playlistContainer = null;
            }

            s.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<SessionEventArgs>(se => se.OnLogoutComplete, s), new SessionEventArgs()));
        }

        private static void MetadataUpdatedCallback(IntPtr sessionPtr)
        {
            logger.Trace("MetadataUpdatedCallback");
            Session s = GetSession(sessionPtr);
            if (s == null)
                return;

            s.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<SessionEventArgs>(se => se.OnMetadataUpdated, s), new SessionEventArgs()));
        }

        private static void ConnectionErrorCallback(IntPtr sessionPtr, Error error)
        {
            logger.Trace("ConnectionErrorCallback: {0}", error);
            Session s = GetSession(sessionPtr);
            if (s == null)
                return;

            s.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<SessionEventArgs>(se => se.OnConnectionError, s), new SessionEventArgs(error)));
        }

        private static void MessageToUserCallback(IntPtr sessionPtr, string message)
        {
            logger.Trace("MessageToUserCallback: {0}", message);
            Session s = GetSession(sessionPtr);
            if (s == null)
                return;

            s.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<SessionEventArgs>(se => se.OnMessageToUser, s), new SessionEventArgs(message)));
        }

        private static void NotifyMainThreadCallback(IntPtr sessionPtr)
        {
            logger.Trace("NotifyMainThreadCallback");
            Session s = GetSession(sessionPtr);
            if (s == null)
                return;

            s.mainThreadNotification.Set();
        }

        private static int MusicDeliveryCallback(IntPtr sessionPtr, IntPtr formatPtr, IntPtr framesPtr, int num_frames)
        {
#if DEBUG
            logger.Trace("MusicDeliveryCallback"); // only on debug builds, cause of performance impact
#endif
            Session s = GetSession(sessionPtr);
            if (s == null)
                return 0;

            byte[] samplesBytes = null;
            AudioFormat format = (AudioFormat)Marshal.PtrToStructure(formatPtr, typeof(AudioFormat));

            if (num_frames > 0)
            {
                samplesBytes = new byte[num_frames * format.Channels * 2];
                Marshal.Copy(framesPtr, samplesBytes, 0, samplesBytes.Length);
            }
            else
                samplesBytes = new byte[0];

            MusicDeliveryEventArgs e = new MusicDeliveryEventArgs(format.Channels, format.SampleRate, samplesBytes, num_frames);
            s.OnMusicDelivery(e);

            return e.ConsumedFrames;
        }

        private static void PlayTokenLostCallback(IntPtr sessionPtr)
        {
            logger.Trace("PlayTokenLostCallback");
            Session s = GetSession(sessionPtr);
            if (s == null)
                return;

            s.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<SessionEventArgs>(se => se.OnPlayTokenLost, s), new SessionEventArgs()));
        }

        private static void LogMessageCallback(IntPtr sessionPtr, string data)
        {
            logger.Trace("LogMessageCallback: {0}", data);
            Session s = GetSession(sessionPtr);
            if (s == null)
                return;

            // Spotify log msgs can contain unprintable chars. Guessing that they control text color on Win32 or something
            data = System.Text.RegularExpressions.Regex.Replace(data, "[\u0000-\u001F]", string.Empty);
            s.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<SessionEventArgs>(se => se.OnLogMessage, s), new SessionEventArgs(data)));
        }

        private static void EndOfTrackCallback(IntPtr sessionPtr)
        {
            logger.Trace("EndOfTrackCallback");
            Session s = GetSession(sessionPtr);
            if (s == null)
                return;

            s.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<SessionEventArgs>(se => se.OnEndOfTrack, s), new SessionEventArgs()));
        }

        private static void StreamingErrorCallback(IntPtr sessionPtr, Error error)
        {
            logger.Trace("StreamingErrorCallback: {0}", error);
            Session s = GetSession(sessionPtr);
            if (s == null)
                return;

            s.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<SessionEventArgs>(se => se.OnStreamingError, s), new SessionEventArgs(error)));
        }

        private static void UserinfoUpdatedCallback(IntPtr sessionPtr)
        {
            logger.Trace("UserinfoUpdatedCallback");
            Session s = GetSession(sessionPtr);
            if (s == null)
                return;

            s.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<SessionEventArgs>(se => se.OnUserinfoUpdated, s), new SessionEventArgs()));
        }

        private static void StartPlaybackCallback(IntPtr sessionPtr)
        {
            logger.Trace("StartPlaybackCallback");
            Session s = GetSession(sessionPtr);
            if (s == null)
                return;

            s.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<SessionEventArgs>(se => se.OnStartPlayback, s), new SessionEventArgs()));
        }

        private static void StopPlaybackCallback(IntPtr sessionPtr)
        {
            logger.Trace("StopPlaybackCallback");
            Session s = GetSession(sessionPtr);
            if (s == null)
                return;

            s.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<SessionEventArgs>(se => se.OnStopPlayback, s), new SessionEventArgs()));
        }

        private static void GetAudioBufferStatsCallback(IntPtr sessionPtr, IntPtr statsPtr)
        {
            logger.Trace("GetAudioBufferStatsCallback");
            Session s = GetSession(sessionPtr);
            // TODO: Implement
        }

        private static void OfflineStatusUpdatedCallback(IntPtr sessionPtr)
        {
            logger.Trace("OfflineStatusUpdatedCallback");
            Session s = GetSession(sessionPtr);
            // TODO: Implement
        }

        private static void OfflineErrorCallback(IntPtr sessionPtr, Error error)
        {
            logger.Trace("OfflineErrorCallback: {0}", error);
            Session s = GetSession(sessionPtr);
            // TODO: Implement
        }

        private static void CredentialsBlobUpdatedCallback(IntPtr sessionPtr, string blob)
        {
            logger.Trace("CredentialsBlogUpdatedCallback: {0}", blob);
            Session s = GetSession(sessionPtr);
            // TODO: Implement
        }

        private static void ConnectionstateUpdatedCallback(IntPtr sessionPtr)
        {
            logger.Trace("ConnectionstateUpdatedCallback");
            Session s = GetSession(sessionPtr);
            // TODO: Implement
        }

        private static void ScrobbleErrorCallback(IntPtr sessionPtr, Error error)
        {
            logger.Trace("ScrobbleErrorCallback: {0}", error);
            Session s = GetSession(sessionPtr);
            // TODO: Implement
        }

        private static void PrivateSessionModeChangedCallback(IntPtr sessionPtr, bool isPrivate)
        {
            logger.Trace("PrivateSessionModeChangedCallback: {0}", isPrivate);
            Session s = GetSession(sessionPtr);
            // TODO: Implement
        }
        #endregion

        #region Session Events
        protected virtual void OnLogMessage(SessionEventArgs args)
        {
            if (this.LogMessage != null)
                this.LogMessage(this, args);
        }

        protected virtual void OnException(SessionEventArgs args)
        {
            if (this.Exception != null)
                this.Exception(this, args);
        }

        protected virtual void OnLoginComplete(SessionEventArgs args)
        {
            if (this.LoginComplete != null)
                this.LoginComplete(this, args);
        }

        protected virtual void OnLogoutComplete(SessionEventArgs args)
        {
            //this.shutdown = true;
            if (this.LogoutComplete != null)
                this.LogoutComplete(this, args);
        }

        protected virtual void OnMetadataUpdated(SessionEventArgs args)
        {
            if (this.MetadataUpdated != null)
                this.MetadataUpdated(this, args);
        }

        protected virtual void OnConnectionError(SessionEventArgs args)
        {
            if (this.ConnectionError != null)
                this.ConnectionError(this, args);
        }

        protected virtual void OnMessageToUser(SessionEventArgs args)
        {
            if (this.MessageToUser != null)
                this.MessageToUser(this, args);
        }

        protected virtual void OnMusicDelivery(MusicDeliveryEventArgs args)
        {
            if (this.MusicDeliver != null)
                this.MusicDeliver(this, args);
        }

        protected virtual void OnPlayTokenLost(SessionEventArgs args)
        {
            if (this.PlayTokenLost != null)
                this.PlayTokenLost(this, args);
        }

        protected virtual void OnEndOfTrack(SessionEventArgs args)
        {
            if (this.EndOfTrack != null)
                this.EndOfTrack(this, args);
        }

        protected virtual void OnStreamingError(SessionEventArgs args)
        {
            if (this.StreamingError != null)
                this.StreamingError(this, args);
        }

        protected virtual void OnUserinfoUpdated(SessionEventArgs args)
        {
            if (this.UserinfoUpdated != null)
                this.UserinfoUpdated(this, args);
        }

        protected virtual void OnStartPlayback(SessionEventArgs args)
        {
            if (this.StartPlayback != null)
                this.StartPlayback(this, args);
        }

        protected virtual void OnStopPlayback(SessionEventArgs args)
        {
            if (this.StopPlayback != null)
                this.StopPlayback(this, args);
        }
        #endregion

        #region Events
        public event SessionEventHandler Exception;
        public event SessionEventHandler LogMessage;
        /*public*/ event SessionEventHandler LoginComplete;
        public event SessionEventHandler LogoutComplete;
        public event SessionEventHandler MetadataUpdated;
        public event SessionEventHandler ConnectionError;
        public event SessionEventHandler MessageToUser;
        public event MusicDeliveryEventHandler MusicDeliver;
        public event SessionEventHandler PlayTokenLost;
        public event SessionEventHandler EndOfTrack;
        public event SessionEventHandler StreamingError;
        public event SessionEventHandler UserinfoUpdated;
        public event SessionEventHandler StartPlayback;
        public event SessionEventHandler StopPlayback;
        #endregion

        #region Properties
        public ConnectionState ConnectionState
        {
            get
            {
                try
                {
                    if (sessionPtr != IntPtr.Zero)
                        lock (libspotify.Mutex)
                            return (ConnectionState)SPSession.connectionstate(sessionPtr);
                    else
                        return ConnectionState.Undefined;
                }
                catch { }
                return ConnectionState.Undefined;
            }
        }
        #endregion

        #region Public Methods
        public Task<bool> Login(string username, string password, bool remember)
        {
            logger.Trace("Login");
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            SessionEventHandler handler = null;
            handler = (s, e) =>
            {
                logger.Trace("Async login completed");
                LoginComplete -= handler;
                tcs.SetResult(e.Status == Error.OK);
            };
            LoginComplete += handler;
            // TODO: Implement blob
            lock (libspotify.Mutex)
            {
                SPSession.login(sessionPtr, username, password, remember, null);
            }
            return tcs.Task;
        }

        public Task Logout()
        {
            logger.Trace("Logout");
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            SessionEventHandler handler = null;
            handler = (s, e) =>
            {
                logger.Trace("Async logout completed");
                LogoutComplete -= handler;
                tcs.SetResult(null);
            };
            LogoutComplete += handler;
            lock (libspotify.Mutex)
            {
                SPSession.logout(sessionPtr);
            }
            return tcs.Task;
        }

        public ISearch Search(string query, int trackOffset, int trackCount, int albumOffset, int albumCount, int artistOffset, int artistCount, int playlistOffset, int playlistCount, SearchType type)
        {
            logger.Trace("Search for {0}. Tracks: {1}-{2}, Albums: {3}-{4}, Artists: {5}-{6}, Playlists: {7}-{8}", query,
                trackOffset, trackOffset + trackCount,
                albumOffset, albumOffset + albumCount,
                artistOffset, artistOffset + artistCount,
                playlistOffset, playlistOffset + playlistCount);
            lock (libspotify.Mutex)
            {
                IntPtr browsePtr = SpotiFire.Search.create(sessionPtr, query, trackOffset, trackCount, albumOffset, albumCount, artistOffset, artistCount,
                    playlistOffset, playlistCount, (int)type, Marshal.GetFunctionPointerForDelegate(SpotifyLib.Search.search_complete), IntPtr.Zero);
                return browsePtr != IntPtr.Zero ? SpotifyLib.Search.Get(this, browsePtr) : null;
            }
        }

        public Error PlayerLoad(ITrack track)
        {
            logger.Trace("PlayerLoad");
            //PlayerUnload();

            Error ret;
            lock (libspotify.Mutex)
                ret = (Error)SPSession.player_load(sessionPtr, Track.GetPointer(track));

            return ret;
        }

        public Error PlayerPlay()
        {
            logger.Trace("PlayerPlay");
            lock (libspotify.Mutex)
                return (Error)SPSession.player_play(sessionPtr, true);
        }

        public Error PlayerPause()
        {
            logger.Trace("PlayerPause");
            lock (libspotify.Mutex)
                return (Error)SPSession.player_play(sessionPtr, false);
        }

        public Error PlayerSeek(int offset)
        {
            logger.Trace("PlayerSeek");
            lock (libspotify.Mutex)
                return (Error)SPSession.player_seek(sessionPtr, offset);
        }

        public Error PlayerSeek(TimeSpan offset)
        {
            return this.PlayerSeek((int)offset.TotalMilliseconds);
        }

        public void PlayerUnload()
        {
            logger.Trace("PlayerUnload");
            lock (libspotify.Mutex)
                SPSession.player_unload(sessionPtr);
        }

        public void SetPrefferedBitrate(BitRate bitrate)
        {
            logger.Trace("SetPrefferedBitrate: {0}", bitrate);
            lock (libspotify.Mutex)
                SPSession.preferred_bitrate(sessionPtr, (int)bitrate);
        }

        public IPlaylist Starred
        {
            get
            {
                logger.Trace("get_Starred");
                lock (libspotify.Mutex)
                    return Playlist.Get(this, SPSession.starred_create(sessionPtr));
            }
        }
        #endregion

        #region IDisposable Methods
        public void Dispose()
        {
            logger.Trace("Dispose");
            if (!disposed)
            {
                disposed = true;
                shutdown = true;
                if (DisposeAll != null)
                    DisposeAll(this, null);
                mainThreadNotification.Set();
                try
                {
                    if (sessionPtr != IntPtr.Zero && !ProcExit)
                    {
                        if (ConnectionState == ConnectionState.LoggedIn)
                            try { SPSession.logout(sessionPtr); }
                            catch { }
                        SPSession.release(sessionPtr);
                    }
                }
                catch { }
            }
        }

        internal event SessionEventHandler DisposeAll;
        #endregion
    }
}

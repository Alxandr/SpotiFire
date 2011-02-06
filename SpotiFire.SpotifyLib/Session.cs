using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Linq.Expressions;
using System.Reflection;

namespace SpotiFire.SpotifyLib
{
    public delegate void SessionEventHandler(Session sender, SessionEventArgs e);
    public delegate void MusicDeliveryEventHandler(Session sender, MusicDeliveryEventArgs e);


    public class Session : IDisposable
    {
        #region Static declarations
        private static Dictionary<IntPtr, Session> sessions = new Dictionary<IntPtr, Session>();
        private static libspotify.sp_session_callbacks callbacks;
        #endregion

        #region Declarations
        internal IntPtr sessionPtr = IntPtr.Zero;
        internal bool shutdown = false;

        private Thread mainThread;
        private Thread eventThread;
        private AutoResetEvent mainThreadNotification = new AutoResetEvent(false);
        private AutoResetEvent eventThreadNotification = new AutoResetEvent(false);

        private Queue<EventWorkItem> eventQueue = new Queue<EventWorkItem>();

        private bool disposed = false;

        private PlaylistContainer playlistContainer = null;

        #endregion

        #region Delegates
        private delegate void logged_in_delegate                (IntPtr sessionPtr, sp_error error);
        private delegate void logged_out_delegate               (IntPtr sessionPtr);
        private delegate void metadata_updated_delegate         (IntPtr sessionPtr);
        private delegate void connection_error_delegate         (IntPtr sessionPtr, sp_error error);
        private delegate void message_to_user_delegate          (IntPtr sessionPtr, string message);
        private delegate void notify_main_thread_delegate       (IntPtr sessionPtr);
        private delegate int  music_delivery_delegate           (IntPtr sessionPtr, IntPtr formatPtr, IntPtr framesPtr, int num_frames);
        private delegate void play_token_lost_delegate          (IntPtr sessionPtr);
        private delegate void log_message_delegate              (IntPtr sessionPtr, string data);
        private delegate void end_of_track_delegate             (IntPtr sessionPtr);
        private delegate void streaming_error_delegate          (IntPtr sessionPtr, sp_error error);
        private delegate void userinfo_updated_delegate         (IntPtr sessionPtr);
        private delegate void start_playback_delegate           (IntPtr sessionPtr);
        private delegate void stop_playback_delegate            (IntPtr sessionPtr);
        private delegate void get_audio_buffer_stats_delegate   (IntPtr sessionPtr, IntPtr statsPtr);
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
        #endregion

        #region Properties
        public PlaylistContainer PlaylistContainer
        {
            get
            {
                return playlistContainer;
            }
        }
        #endregion

        #region Constructor
        private Session(byte[] applicationKey, string cacheLocation, string settingsLocation, string userAgent)
        {
            libspotify.sp_session_config config = new libspotify.sp_session_config();

            config.api_version = libspotify.SPOTIFY_API_VERSION;
            config.cache_location = cacheLocation;
            config.settings_location = settingsLocation;
            config.user_agent = userAgent;
            config.tiny_settings = false;

            int size = Marshal.SizeOf(callbacks);
            config.callbacks = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(callbacks, config.callbacks, true);

            config.application_key = IntPtr.Zero;

            try
            {
                config.application_key = Marshal.AllocHGlobal(applicationKey.Length);
                Marshal.Copy(applicationKey, 0, config.application_key, applicationKey.Length);

                lock (libspotify.Mutex)
                    config.application_key_size = applicationKey.Length;

                sessionPtr = IntPtr.Zero;
                sp_error res = libspotify.sp_session_create(ref config, out sessionPtr);

                if (res != sp_error.OK)
                    throw new SpotifyException(res);

                mainThread = new Thread(new ThreadStart(MainThread));
                mainThread.IsBackground = true;
                mainThread.Start();

                eventThread = new Thread(new ThreadStart(EventThread));
                eventThread.IsBackground = true;
                eventThread.Start();
            }
            finally
            {
                if (config.application_key != IntPtr.Zero)
                    Marshal.FreeHGlobal(config.application_key);
            }
        }

        static Session()
        {
            lock (libspotify.Mutex)
            {
                callbacks = new libspotify.sp_session_callbacks();
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
            }
        }

        public static Session Create(byte[] applicationKey, string cacheLocation, string settingsLocation, string userAgent)
        {
            lock(libspotify.Mutex)
            {
                if(sessions.Count > 0)
                    throw new InvalidOperationException("libspotify can only handle one session at the moment");

                Session instance = new Session(applicationKey, cacheLocation, settingsLocation, userAgent);
                sessions.Add(instance.sessionPtr, instance);
                return instance;
            }
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
                            libspotify.sp_session_process_events(sessionPtr, out waitTime);
                        }
                        catch
                        {
                            waitTime = 1000;
                        }
                    }
                }
                while (waitTime == 0);
            }
        }

        private void EventThread()
        {
            List<EventWorkItem> localList = new List<EventWorkItem>();
            while (!shutdown)
            {
                eventThreadNotification.WaitOne();
                lock (eventQueue)
                {
                    while (eventQueue.Count > 0)
                        localList.Add(eventQueue.Dequeue());
                }

                foreach (var workerItem in localList)
                {
                    try
                    {
                        workerItem.Execute();
                    }
                    catch (Exception ex)
                    {
                        OnException(new SessionEventArgs(ex.ToString()));
                    }
                }

                localList.Clear();
            }
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

        private static Delegate CreateDelegate<T>(Expression<Func<Session, Action<T>>> expr, Session s) where T : EventArgs
        {
            return expr.Compile().Invoke(s);
        }

        internal void EnqueueEventWorkItem(EventWorkItem eventWorkerItem)
        {
            lock (eventQueue)
            {
                eventQueue.Enqueue(eventWorkerItem);
            }

            eventThreadNotification.Set();
        }
        #endregion

        #region Spotify Event Handler Functions
        private static void LoggedInCallback(IntPtr sessionPtr, sp_error error)
        {
            Session s = GetSession(sessionPtr);
            if (s == null)
                return;

            // TODO: Implement Playlists.
            if (s.ConnectionState == sp_connectionstate.LOGGED_IN && error == sp_error.OK)
                s.playlistContainer = new PlaylistContainer(s, libspotify.sp_session_playlistcontainer(sessionPtr));

            s.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<SessionEventArgs>(se => se.OnLoginComplete, s), new SessionEventArgs(error)));
        }

        private static void LoggedOutCallback(IntPtr sessionPtr)
        {
            Session s = GetSession(sessionPtr);
            if (s == null)
                return;

            //if (s.PlaylistContainer != null)
            //{
            //    s.playlistContainer.Dispose();
            //    s.playlistContainer = null;
            //}

            s.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<SessionEventArgs>(se => se.OnLogoutComplete, s), new SessionEventArgs()));
        }

        private static void MetadataUpdatedCallback(IntPtr sessionPtr)
        {
            Session s = GetSession(sessionPtr);
            if (s == null)
                return;

            s.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<SessionEventArgs>(se => se.OnMetadataUpdated, s), new SessionEventArgs()));
        }

        private static void ConnectionErrorCallback(IntPtr sessionPtr, sp_error error)
        {
            Session s = GetSession(sessionPtr);
            if (s == null)
                return;

            s.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<SessionEventArgs>(se => se.OnConnectionError, s), new SessionEventArgs(error)));
        }

        private static void MessageToUserCallback(IntPtr sessionPtr, string message)
        {
            Session s = GetSession(sessionPtr);
            if (s == null)
                return;

            s.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<SessionEventArgs>(se => se.OnMessageToUser, s), new SessionEventArgs(message)));
        }

        private static void NotifyMainThreadCallback(IntPtr sessionPtr)
        {
            Session s = GetSession(sessionPtr);
            if (s == null)
                return;

            s.mainThreadNotification.Set();
        }

        private static int MusicDeliveryCallback(IntPtr sessionPtr, IntPtr formatPtr, IntPtr framesPtr, int num_frames)
        {
            Session s = GetSession(sessionPtr);
            if (s == null)
                return 0;

            byte[] samplesBytes = null;
            libspotify.sp_audioformat format = (libspotify.sp_audioformat)Marshal.PtrToStructure(formatPtr, typeof(libspotify.sp_audioformat));

            if (num_frames > 0)
            {
                samplesBytes = new byte[num_frames * format.channels * 2];
                Marshal.Copy(framesPtr, samplesBytes, 0, samplesBytes.Length);
            }
            else
                samplesBytes = new byte[0];

            MusicDeliveryEventArgs e = new MusicDeliveryEventArgs(format.channels, format.sample_rate, samplesBytes, num_frames);
            s.OnMusicDelivery(e);

            return e.ConsumedFrames;
        }

        private static void PlayTokenLostCallback(IntPtr sessionPtr)
        {
            Session s = GetSession(sessionPtr);
            if (s == null)
                return;

            s.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<SessionEventArgs>(se => se.OnPlayTokenLost, s), new SessionEventArgs()));
        }

        private static void LogMessageCallback(IntPtr sessionPtr, string data)
        {
            Session s = GetSession(sessionPtr);
            if (s == null)
                return;

            // Spotify log msgs can contain unprintable chars. Guessing that they control text color on Win32 or something
            data = System.Text.RegularExpressions.Regex.Replace(data, "[\u0000-\u001F]", string.Empty);
            s.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<SessionEventArgs>(se => se.OnLogMessage, s), new SessionEventArgs(data)));
        }

        private static void EndOfTrackCallback(IntPtr sessionPtr)
        {
            Session s = GetSession(sessionPtr);
            if (s == null)
                return;

            s.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<SessionEventArgs>(se => se.OnEndOfTrack, s), new SessionEventArgs()));
        }

        private static void StreamingErrorCallback(IntPtr sessionPtr, sp_error error)
        {
            Session s = GetSession(sessionPtr);
            if (s == null)
                return;

            s.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<SessionEventArgs>(se => se.OnStreamingError, s), new SessionEventArgs(error)));
        }

        private static void UserinfoUpdatedCallback(IntPtr sessionPtr)
        {
            Session s = GetSession(sessionPtr);
            if (s == null)
                return;

            s.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<SessionEventArgs>(se => se.OnUserinfoUpdated, s), new SessionEventArgs()));
        }

        private static void StartPlaybackCallback(IntPtr sessionPtr)
        {
            Session s = GetSession(sessionPtr);
            if (s == null)
                return;

            s.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<SessionEventArgs>(se => se.OnStartPlayback, s), new SessionEventArgs()));
        }

        private static void StopPlaybackCallback(IntPtr sessionPtr)
        {
            Session s = GetSession(sessionPtr);
            if (s == null)
                return;

            s.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<SessionEventArgs>(se => se.OnStopPlayback, s), new SessionEventArgs()));
        }

        private static void GetAudioBufferStatsCallback(IntPtr sessionPtr, IntPtr statsPtr)
        {
            //throw new NotImplementedException("GetAudioBufferStatsCallback");
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
            this.shutdown = true;
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
        public event SessionEventHandler LoginComplete;
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
        public sp_connectionstate ConnectionState
        {
            get
            {
                try
                {
                    if (sessionPtr != IntPtr.Zero)
                        lock (libspotify.Mutex)
                            return libspotify.sp_session_connectionstate(sessionPtr);
                    else
                        return sp_connectionstate.UNDEFINED;
                }
                catch { }
                return sp_connectionstate.UNDEFINED;
            }
        }
        #endregion

        #region Public Methods
        public void Login(string username, string password)
        {
            lock (libspotify.Mutex)
            {
                sp_error res = libspotify.sp_session_login(sessionPtr, username, password);

                if (res != sp_error.OK)
                    throw new SpotifyException(res);
            }
        }

        public void Logout()
        {
            lock (libspotify.Mutex)
            {
                sp_error res = libspotify.sp_session_logout(sessionPtr);

                if (res != sp_error.OK)
                    throw new SpotifyException(res);
            }
        }

        public Search Search(string query, int trackOffset, int trackCount, int albumOffset, int albumCount, int artistOffset, int artistCount)
        {
            lock (libspotify.Mutex)
            {
                IntPtr browsePtr = libspotify.sp_search_create(sessionPtr, query, trackOffset, trackCount, albumOffset, albumCount, artistOffset, artistCount,
                    Marshal.GetFunctionPointerForDelegate(SpotifyLib.Search.search_complete), IntPtr.Zero);
                return browsePtr != IntPtr.Zero ? new Search(this, browsePtr) : null;
            }
        }

        public sp_error PlayerLoad(ITrack track)
        {
            PlayerUnload();

            sp_error ret;
            lock (libspotify.Mutex)
                ret = libspotify.sp_session_player_load(sessionPtr, Track.GetPointer(track));

            return ret;
        }

        public sp_error PlayerPlay()
        {
            lock (libspotify.Mutex)
                return libspotify.sp_session_player_play(sessionPtr, true);
        }

        public sp_error PlayerPause()
        {
            lock (libspotify.Mutex)
                return libspotify.sp_session_player_play(sessionPtr, false);
        }

        public void PlayerUnload()
        {
            lock (libspotify.Mutex)
                libspotify.sp_session_player_unload(sessionPtr);
        }

        public void SetPrefferedBitrate(sp_bitrate bitrate)
        {
            lock (libspotify.Mutex)
                libspotify.sp_session_preferred_bitrate(sessionPtr, bitrate);
        }
        #endregion

        #region IDisposable Methods
        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                try
                {
                    if (sessionPtr != IntPtr.Zero)
                    {
                        try { libspotify.sp_session_logout(sessionPtr); }
                        catch { }
                        libspotify.sp_session_release(sessionPtr);
                    }
                }
                catch { }
            }
        }
        #endregion

        public override int GetHashCode()
        {
            return sessionPtr.ToInt32();
        }
    }
}

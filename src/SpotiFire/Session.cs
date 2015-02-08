using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace SpotiFire
{
    public sealed class Session : NativeObject
    {
        readonly static sp_session_callbacks callbacks;
        readonly static ConcurrentDictionary<IntPtr, Session> _sessions = new ConcurrentDictionary<IntPtr, Session>();

        static Session()
        {
            callbacks = new sp_session_callbacks();
            try
            {
                callbacks.logged_in = Marshal.GetFunctionPointerForDelegate((libspotify.logged_in)Session.LoggedInCallback);
                //callbacks.logged_out = Marshal.GetFunctionPointerForDelegate(logged_out);
                //callbacks.metadata_updated = Marshal.GetFunctionPointerForDelegate(metadata_updated);
                //callbacks.connection_error = Marshal.GetFunctionPointerForDelegate(connection_error);
                //callbacks.message_to_user = Marshal.GetFunctionPointerForDelegate(message_to_user);
                callbacks.notify_main_thread = Marshal.GetFunctionPointerForDelegate((libspotify.session_callback)Session.NotifyMainThreadCallback);
                //callbacks.music_delivery = Marshal.GetFunctionPointerForDelegate(music_delivery);
                //callbacks.play_token_lost = Marshal.GetFunctionPointerForDelegate(play_token_lost);
                //callbacks.log_message = Marshal.GetFunctionPointerForDelegate(log_message);
                //callbacks.end_of_track = Marshal.GetFunctionPointerForDelegate(end_of_track);
                //callbacks.streaming_error = Marshal.GetFunctionPointerForDelegate(streaming_error);
                //callbacks.userinfo_updated = Marshal.GetFunctionPointerForDelegate(userinfo_updated);
                //callbacks.start_playback = Marshal.GetFunctionPointerForDelegate(start_playback);
                //callbacks.stop_playback = Marshal.GetFunctionPointerForDelegate(stop_playback);
                //callbacks.get_audio_buffer_stats = Marshal.GetFunctionPointerForDelegate(get_audio_buffer_stats);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                Serilog.Log.Fatal(e, "Error during initialization of type");
                throw;
            }
        }

        internal static Task<Session> Create(byte[] applicationKey, string cacheLocation, string settingsLocation, string userAgent, ILogger logger)
        {
            return Task.Run(() =>
            {
                logger.Information("In Task.Run");
                var bound = new List<IntPtr>();
                var config = new sp_session_config();

                config.api_version = libspotify.SPOTIFY_API_VERSION;
                config.cache_location = bound.AddAndReturn(MakeString(cacheLocation));
                config.settings_location = bound.AddAndReturn(MakeString(settingsLocation));
                config.user_agent = bound.AddAndReturn(MakeString(userAgent));
                config.callbacks = bound.AddAndReturn(Marshal.AllocHGlobal(Marshal.SizeOf(callbacks)));
                Marshal.StructureToPtr(callbacks, config.callbacks, true);
                config.application_key = IntPtr.Zero;

                try
                {
                    config.application_key = Marshal.AllocHGlobal(applicationKey.Length);
                    Marshal.Copy(applicationKey, 0, config.application_key, applicationKey.Length);

                    config.application_key_size = applicationKey.Length;

                    var sessionPtr = IntPtr.Zero;
                    var configPtr = bound.AddAndReturn(Marshal.AllocHGlobal(Marshal.SizeOf(config)));
                    Marshal.StructureToPtr(config, configPtr, true);

                    logger.Information("Running sp_session_create");
                    lock (libspotify.mutex)
                        libspotify.sp_session_create(configPtr, out sessionPtr).EnsureSuccess();

                    return new Session(sessionPtr, logger);
                }
                finally
                {
                    if (config.application_key != IntPtr.Zero)
                        Marshal.FreeHGlobal(config.application_key);
                }
            });
        }

        readonly AutoResetEvent _mainThreadNotifier = new AutoResetEvent(initialState: false);
        readonly Thread _mainThread;
        readonly object _lock = new object();
        volatile bool _running = true;
        TaskCompletionSource<object> _login;
        TaskCompletionSource<object> _logout;

        internal Session(IntPtr ptr, ILogger logger)
            : base(ptr, logger)
        {
            if (!_sessions.TryAdd(ptr, this))
                throw new InvalidOperationException("Multiple sessions on same pointer");

            _mainThread = new Thread(() =>
            {
                int waitTime = 0;
                while (_running)
                {
                    _mainThreadNotifier.WaitOne(waitTime, true);
                    do
                    {
                        lock (libspotify.mutex)
                        {
                            try
                            {
                                libspotify.sp_session_process_events(NativePtr, out waitTime).EnsureSuccess();
                            }
                            catch (Exception e)
                            {
                                Log.Error(e, "Error when processing session events");
                            }
                        }
                    } while (waitTime == 0);
                }
            });
            _mainThread.IsBackground = true;
            _mainThread.Start();
        }

        public Task Login(string username, string password, bool rememberMe, string blob = null)
        {
            lock (_lock)
            {
                if (_login != null)
                    _login.SetCanceled();
                _login = new TaskCompletionSource<object>();
            }

            using (var context = new NativeContext())
            {
                lock (libspotify.mutex)
                    libspotify.sp_session_login(
                        NativePtr,
                        context.String(username),
                        context.String(password),
                        rememberMe,
                        context.String(blob)).EnsureSuccess();
            }

            return _login.Task;
        }

        protected override void Free()
        {
            Session ignored;
            _running = false;
            _mainThreadNotifier.Set();
            _mainThread.Join();
            _sessions.TryRemove(NativePtr, out ignored);
            lock (libspotify.mutex)
                libspotify.sp_session_release(NativePtr).EnsureSuccess();
        }

        #region Handlers
        private void OnLoggedIn(sp_error error)
        {
            Log.LogMethod()(error);
            TaskCompletionSource<object> login;
            lock (_lock)
            {
                login = _login;
                _login = null;
            }

            if (login != null)
            {
                try
                {
                    error.EnsureSuccess();
                    login.SetResult(true);
                }
                catch (Exception e)
                {
                    login.SetException(e);
                }
            }
            else
            {
                error.EnsureSuccess();
            }
        }

        private void NotifyMainThread()
        {
            Log.LogMethod()();
            _mainThreadNotifier.Set();
        }
        #endregion

        #region Static Handlers
        private static void LoggedInCallback(IntPtr sessionPtr, sp_error error)
        {
            Session s;
            if (_sessions.TryGetValue(sessionPtr, out s))
            {
                s.OnLoggedIn(error);
            }
        }

        private static void NotifyMainThreadCallback(IntPtr sessionPtr)
        {
            Session s;
            if (_sessions.TryGetValue(sessionPtr, out s))
            {
                s.NotifyMainThread();
            }
        }
        #endregion
    }
}
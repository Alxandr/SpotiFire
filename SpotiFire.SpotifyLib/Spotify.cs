using System;
using System.Threading.Tasks;

namespace SpotiFire.SpotifyLib
{
    public static class Spotify
    {
        static readonly object _lock = new object();
        static ISession _session;
        static Task<ISession> _task;

        public static Task<ISession> CreateSession(byte[] applicationKey, string cacheLocation, string settingsLocation, string userAgent)
        {
            lock (_lock)
            {
                if(_task != null || _session != null)
                    throw new InvalidOperationException("Can only have one session in process");

                _task = CreateSessionInternal(applicationKey, cacheLocation, settingsLocation, userAgent);
            }

            return _task;
        }

        public static Task<ISession> Task
        {
            get
            {
                return _task;
            }
        }

        public static ISession Session
        {
            get
            {
                if (_task == null)
                    return null;

                if (_session == null)
                {
                    _task.Wait();
                }

                return _session;
            }
        }

        internal static async Task<ISession> CreateSessionInternal(byte[] applicationKey, string cacheLocation, string settingsLocation, string userAgent)
        {
            _session = await SpotiFire.SpotifyLib.Session.Create(applicationKey, cacheLocation, settingsLocation, userAgent);
            return _session;
        }
    }
}

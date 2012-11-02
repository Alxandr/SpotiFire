using System;
using System.Threading.Tasks;

namespace SpotiFire
{
    public static class Spotify
    {
        static readonly object _lock = new object();
        static Session _session;
        static Task<Session> _task;

        public static Task<Session> CreateSession(byte[] applicationKey, string cacheLocation, string settingsLocation, string userAgent)
        {
            lock (_lock)
            {
                if(_task != null || _session != null)
                    throw new InvalidOperationException("Can only have one session in process");

                _task = CreateSessionInternal(applicationKey, cacheLocation, settingsLocation, userAgent);
            }

            return _task;
        }

        public static Task<Session> Task
        {
            get
            {
                return _task;
            }
        }

        public static Session Session
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

        internal static async Task<Session> CreateSessionInternal(byte[] applicationKey, string cacheLocation, string settingsLocation, string userAgent)
        {
            _session = await Session.Create(applicationKey, cacheLocation, settingsLocation, userAgent);
            return _session;
        }
    }
}

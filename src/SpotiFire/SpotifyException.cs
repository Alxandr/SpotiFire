using System;

namespace SpotiFire
{
    public class SpotifyException : Exception
    {
        readonly sp_error _error;

        internal SpotifyException(sp_error error)
        {
            _error = error;
        }

        public override string Message
        {
            get
            {
                lock (libspotify.mutex)
                {
                    return NativeObject.GetString(libspotify.sp_error_message(_error));
                }
            }
        }
    }
}
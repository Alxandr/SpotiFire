using System;
using System.Threading.Tasks;

namespace SpotiFire
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   The Spotify-class. </summary>
    ///
    /// <remarks>   This class is the starting-point for all interaction with Spotify.
    ///             From here you can create a <see cref="SpotiFire.Session"/> object that
    ///             is used to fetch playlists and play tracks.</remarks>
    /// <example>   Create a new spotify-session:
    ///             <code>
    ///                 <code lang="cs"><![CDATA[
    /// using System;
    /// using SpotiFire;
    /// 
    /// public class SpotifyClient
    /// {
    ///     const string CLIENT_NAME = "MyClient";
    ///     static byte[] key = new byte[] { /* insert key here */ };
    ///     static string cache = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), CLIENT_NAME, "cache");
    ///     static string settings = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), CLIENT_NAME, "settings");
    ///     static string userAgent = CLIENT_NAME;
    /// 
    ///     public static void Main()
    ///     {
    ///         Run().Wait();
    ///     }
    ///     
    ///     private static async Task Run()
    ///     {
    ///         var session = await Spotify.CreateSession(key, cache, settings, userAgent);
    ///         Console.WriteLine("Enter username and password (a line for each)");
    ///         await session.Login(Console.ReadLine(), Console.ReadLine(), false);
    ///         
    ///         // search playlists, play music etc
    ///         
    ///         await session.Logout();
    ///     }
    /// }
    ///                 ]]></code>
    ///                 <code lang="vb"><![CDATA[
    /// Imports System
    /// Imports SpotiFire
    ///
    /// Public Class SpotifyClient
    ///	Const CLIENT_NAME As String = "MyClient"
    ///		' insert key here
    ///	Shared key As Byte() = New Byte() {}
    ///	Shared cache As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), CLIENT_NAME, "cache")
    ///	Shared settings As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), CLIENT_NAME, "settings")
    ///	Shared userAgent As String = CLIENT_NAME
    ///
    ///	Public Shared Sub Main()
    ///		Run().Wait()
    ///	End Sub
    ///
    ///	Private Shared Function Run() As Task
    ///		Dim session = Await Spotify.CreateSession(key, cache, settings, userAgent)
    ///		Console.WriteLine("Enter username and password (a line for each)")
    ///		Await session.Login(Console.ReadLine(), Console.ReadLine(), False)
    ///
    ///		' search playlists, play music etc
    ///
    ///		Await session.Logout()
    ///	End Function
    /// End Class
    ///                 ]]></code>
    ///             </code></example>
    ///-------------------------------------------------------------------------------------------------
    public static class Spotify
    {
        static readonly object _lock = new object();
        static Session _session;
        static Task<Session> _task;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Creates a session. </summary>
        ///
        /// <remarks>   This method should only be called once per process. Also, the application should
        ///             make sure there never is more than one instance running at once. </remarks>
        ///
        /// <exception cref="InvalidOperationException"> Thrown when a session has already been created. </exception>
        ///
        /// <param name="applicationKey">   The application key. </param>
        /// <param name="cacheLocation">    The cache location. </param>
        /// <param name="settingsLocation"> The settings location. </param>
        /// <param name="userAgent">        The user agent. </param>
        ///
        /// <returns>   A new spotify-session. </returns>
        ///-------------------------------------------------------------------------------------------------
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

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Get's the pending task that represents the creation of a Session object. </summary>
        ///
        /// <value> The task. </value>
        ///-------------------------------------------------------------------------------------------------
        public static Task<Session> Task
        {
            get
            {
                return _task;
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the current session. </summary>
        ///
        /// <value> The session. </value>
        ///-------------------------------------------------------------------------------------------------
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

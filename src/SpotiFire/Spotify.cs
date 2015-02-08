using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;

namespace SpotiFire
{
    public static class Spotify
    {
        static readonly object _lock = new object();
        static bool _created = false;

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
        public static Task<Session> CreateSession(byte[] applicationKey, string cacheLocation, string settingsLocation, string userAgent, ILogger logger = null)
        {
            lock (_lock)
            {
                if(_created)
                    throw new InvalidOperationException("Can only have one session in process");

                _created = true;
            }

            if(logger == null)
            {
                logger = new LoggerConfiguration()
                    .WriteTo.ColoredConsole(LogEventLevel.Verbose)
                    .CreateLogger();
            }

            logger.Information("Creating session");
            return CreateSessionInternal(applicationKey, cacheLocation, settingsLocation, userAgent, logger);
        }

        internal static async Task<Session> CreateSessionInternal(byte[] applicationKey, string cacheLocation, string settingsLocation, string userAgent, ILogger logger)
        {
            var appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpotiFire", "lib");
            if(!Directory.Exists(appDir))
            {
                logger.Information("Unpacking native assemblies");
                Directory.CreateDirectory(appDir);

                var asm = Assembly.GetExecutingAssembly();
                var resources = asm.GetManifestResourceNames()
                    .Where(n => n.StartsWith("libspotify/"));

                foreach(var lib in resources)
                {
                    var fName = Path.Combine(appDir, Path.GetFileName(lib));
                    using (var resourceStream = asm.GetManifestResourceStream(lib))
                    using (var fileStream = File.Create(fName))
                        await resourceStream.CopyToAsync(fileStream);
                }
            }

            logger.Information("Setting path");
            var path = Environment.GetEnvironmentVariable("PATH");
            path = path + ";" + appDir;
            Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.Process);

            logger.Information("Running Session.Create");
            return await Session.Create(applicationKey, cacheLocation, settingsLocation, userAgent, logger);
        }
    }
}
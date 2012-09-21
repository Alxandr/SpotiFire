using System.Threading.Tasks;

namespace SpotiFire.SpotifyLib
{
    public static class Spotify
    {
        public static async Task<ISession> CreateSession(byte[] applicationKey, string cacheLocation, string settingsLocation, string userAgent)
        {
            return await Session.Create(applicationKey, cacheLocation, settingsLocation, userAgent);
        }
    }
}

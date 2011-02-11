
namespace SpotiFire.SpotifyLib
{
    public static class Spotify
    {
        public static ISession CreateSession(byte[] applicationKey, string cacheLocation, string settingsLocation, string userAgent)
        {
            return Session.Create(applicationKey, cacheLocation, settingsLocation, userAgent);
        }

        public static IImage GetImageFromId(ISession session, string id)
        {
            return Image.FromId(session, id);
        }
    }
}

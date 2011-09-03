using System;
namespace SpotiFire.SpotifyLib
{
    public interface IArtist : ISpotifyObject, IDisposable
    {
        bool IsLoaded { get; }
        string Name { get; }
        IArtistBrowse Browse();
    }
}

using System;
namespace SpotiFire.SpotifyLib
{
    public interface IArtist : IDisposable
    {
        bool IsLoaded { get; }
        string Name { get; }
    }
}

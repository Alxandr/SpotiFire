using System;
namespace SpotiFire.SpotifyLib
{
    public interface ITrack : ISpotifyObject, IDisposable
    {
        IAlbum Album { get; }
        IArray<IArtist> Artists { get; }
        int Disc { get; }
        int Duration { get; }
        sp_error Error { get; }
        int Index { get; }
        bool IsAvailable { get; }
        bool IsLoaded { get; }
        string Name { get; }
        int Popularity { get; }
    }
}

using System;
namespace SpotiFire.SpotifyLib
{
    public interface ITrack : ISpotifyObject, IAsyncLoaded
    {
        IAlbum Album { get; }
        IArray<IArtist> Artists { get; }
        int Disc { get; }
        TimeSpan Duration { get; }
        Error Error { get; }
        int Index { get; }
        TrackAvailability Availability { get; }
        bool IsStarred { get; set; }
        string Name { get; }
        int Popularity { get; }
    }
}

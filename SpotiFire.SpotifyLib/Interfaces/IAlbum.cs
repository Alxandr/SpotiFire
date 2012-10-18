using System;
namespace SpotiFire.SpotifyLib
{
    public interface IAlbum : ISpotifyObject, IAsyncLoaded
    {
        IArtist Artist { get; }
        string CoverId { get; }
        bool IsAvailable { get; }
        bool IsLoaded { get; }
        string Name { get; }
        AlbumType Type { get; }
        int Year { get; }
        IAlbumBrowse Browse();
    }
}

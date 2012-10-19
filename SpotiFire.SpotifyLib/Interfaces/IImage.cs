using System;
namespace SpotiFire
{
    public interface IImage : ISpotifyObject, IDisposable
    {
        byte[] Data { get; }
        Error Error { get; }
        ImageFormat Format { get; }
        string ImageId { get; }
        bool IsLoaded { get; }
        event ImageEventHandler Loaded;
    }
}

using System;

namespace SpotiFire
{
    public interface ISpotifyObject : IDisposable
    {
        ISession Session { get; }
    }
}

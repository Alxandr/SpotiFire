using System;

namespace SpotiFire
{
    public interface ITrackAndOffset
    {
        ITrack Track { get; }
        TimeSpan Offset { get; }
    }
}

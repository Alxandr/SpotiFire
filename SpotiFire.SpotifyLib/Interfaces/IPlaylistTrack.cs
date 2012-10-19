using System;

namespace SpotiFire
{
    public interface IPlaylistTrack : ITrack
    {
        DateTime CreateTime { get; }
        //IUser Creator { get; }
        bool Seen { get; }
    }
}

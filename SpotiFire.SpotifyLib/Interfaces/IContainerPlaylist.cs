
namespace SpotiFire.SpotifyLib
{
    public interface IContainerPlaylist : IPlaylist
    {
        PlaylistType Type { get; }
    }
}


namespace SpotiFire.SpotifyLib
{
    public interface IPlaylist : ISpotifyObject
    {
        string Name { get; set; }
        sp_playlist_type Type { get; }
    }
}

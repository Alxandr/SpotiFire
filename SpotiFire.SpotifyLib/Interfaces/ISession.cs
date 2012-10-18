using System;
using System.Threading.Tasks;

namespace SpotiFire.SpotifyLib
{
    public interface ISession : IDisposable
    {
        event SessionEventHandler ConnectionError;
        ConnectionState ConnectionState { get; }
        event SessionEventHandler EndOfTrack;
        event SessionEventHandler Exception;
        Task<bool> Login(string username, string password, bool remember);
        //event SessionEventHandler LoginComplete;
        event SessionEventHandler LogMessage;
        Task Logout();
        event SessionEventHandler LogoutComplete;
        event SessionEventHandler MessageToUser;
        event SessionEventHandler MetadataUpdated;
        event MusicDeliveryEventHandler MusicDeliver;
        Error PlayerLoad(ITrack track);
        Error PlayerPause();
        Error PlayerPlay();
        Error PlayerSeek(int offset);
        Error PlayerSeek(TimeSpan offset);
        void PlayerUnload();
        IPlaylistContainer PlaylistContainer { get; }
        IPlaylist Starred { get; }
        event SessionEventHandler PlayTokenLost;
        ISearch Search(string query, int trackOffset, int trackCount, int albumOffset, int albumCount, int artistOffset, int artistCount, int playlistOffset, int playlistCount, SearchType type);
        void SetPrefferedBitrate(BitRate bitrate);
        event SessionEventHandler StartPlayback;
        event SessionEventHandler StopPlayback;
        event SessionEventHandler StreamingError;
        event SessionEventHandler UserinfoUpdated;
    }
}

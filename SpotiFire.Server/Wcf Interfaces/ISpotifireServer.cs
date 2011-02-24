
using System;
using System.ServiceModel;

namespace SpotiFire.Server
{
    [ServiceContract(CallbackContract = typeof(ISpotifireClient), SessionMode = SessionMode.Required, Name = "Spotify")]
    [ServiceKnownType(typeof(PlaylistType))]
    [ServiceKnownType(typeof(Playlist))]
    [ServiceKnownType(typeof(Track))]
    public interface ISpotifireServer
    {
        [OperationContract(IsOneWay = false, IsInitiating = true)]
        bool Authenticate(string password);

        [OperationContract(IsOneWay = true, IsInitiating = false)]
        void Login(string username, string password);

        [OperationContract(IsOneWay = true, IsInitiating = false)]
        void Pong();

        [OperationContract(IsOneWay = false, IsInitiating = false)]
        Playlist[] GetPlaylists();

        [OperationContract(IsOneWay = false, IsInitiating = false)]
        Track[] GetPlaylistTracks(Guid id);

        [OperationContract(IsOneWay = true, IsInitiating = false)]
        void PlayPlaylistTrack(Guid playlistId, int position);
    }
}


using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SpotiFire.Server
{
    [ServiceContract(CallbackContract = typeof(ISpotifireClient), SessionMode = SessionMode.Required, Name = "Spotify")]
    [ServiceKnownType(typeof(PlaylistType))]
    [ServiceKnownType(typeof(Playlist))]
    [ServiceKnownType(typeof(Track))]
    [ServiceKnownType(typeof(Artist))]
    [ServiceKnownType(typeof(AuthenticationStatus))]
    [ServiceKnownType(typeof(SpotifyStatus))]
    public interface ISpotifireServer
    {
        [OperationContract(IsOneWay = false, IsInitiating = true)]
        AuthenticationStatus Authenticate(string password);

        [OperationContract(IsOneWay = false, IsInitiating = false)]
        bool Login(string username, string password);

        [OperationContract(IsOneWay = true, IsInitiating = false)]
        void Pong();

        [OperationContract(IsOneWay = false, IsInitiating = false)]
        IEnumerable<Playlist> GetPlaylists();

        [OperationContract(IsOneWay = false, IsInitiating = false)]
        IEnumerable<Track> GetPlaylistTracks(Guid id);

        [OperationContract(IsOneWay = true, IsInitiating = false)]
        void PlayPlaylistTrack(Guid playlistId, int position);

        [OperationContract(IsOneWay = true, IsInitiating = false)]
        void SetShuffle(bool random);

        [OperationContract(IsOneWay = true, IsInitiating = false)]
        void SetRepeat(bool repeat);

        [OperationContract(IsOneWay = false, IsInitiating = false)]
        SpotifyStatus GetStatus();

        [OperationContract(IsOneWay = true, IsInitiating = false, IsTerminating = true)]
        void Exit();

        [OperationContract(IsOneWay = true, IsInitiating = false)]
        void SetVolume(int volume);

        [OperationContract(IsOneWay = true, IsInitiating = false)]
        void PlayPause();
    }
}

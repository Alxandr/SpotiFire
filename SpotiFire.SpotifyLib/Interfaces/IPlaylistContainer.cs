namespace SpotiFire.SpotifyLib
{
    public interface IPlaylistContainer : ISpotifyObject
    {
        //IUser User {get;}
        IEditableArray<IContainerPlaylist> Playlists { get; }
        bool IsLoaded { get; }
        event PlaylistContainerHandler Loaded;
        event PlaylistContainerHandler<PlaylistEventArgs> PlaylistAdded;
        event PlaylistContainerHandler<PlaylistMovedEventArgs> PlaylistMoved;
        event PlaylistContainerHandler<PlaylistEventArgs> PlaylistRemoved;
    }
}

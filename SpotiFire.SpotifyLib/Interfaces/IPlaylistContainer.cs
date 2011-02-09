namespace SpotiFire.SpotifyLib
{
    public interface IPlaylistContainer : ISpotifyObject
    {
        //IUser User {get;}
        IEditableArray<IPlaylist> Playlists { get; }
        event PlaylistContainerHandler Loaded;
        event PlaylistContainerHandler<PlaylistEventArgs> PlaylistAdded;
        event PlaylistContainerHandler<PlaylistMovedEventArgs> PlaylistMoved;
        event PlaylistContainerHandler<PlaylistEventArgs> PlaylistRemoved;
    }
}

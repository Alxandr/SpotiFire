
namespace SpotiFire.SpotifyLib
{
    public interface IPlaylist : ISpotifyObject
    {
        string Name { get; set; }
        IEditableArray<ITrack> Tracks { get; }
        string ImageId { get; }

        event PlaylistEventHandler<TracksAddedEventArgs> TracksAdded;
        event PlaylistEventHandler<TracksEventArgs> TracksRemoved;
        event PlaylistEventHandler<TracksMovedEventArgs> TracksMoved;
        event PlaylistEventHandler Renamed;
        event PlaylistEventHandler StateChanged;
        event PlaylistEventHandler<PlaylistUpdateEventArgs> UpdateInProgress;
        event PlaylistEventHandler MetadataUpdated;
        event PlaylistEventHandler<TrackCreatedChangedEventArgs> TrackCreatedChanged;
        event PlaylistEventHandler<TrackSeenEventArgs> TrackSeenChanged;
        event PlaylistEventHandler<DescriptionEventArgs> DescriptionChanged;
        event PlaylistEventHandler<ImageEventArgs> ImageChanged;
    }
}

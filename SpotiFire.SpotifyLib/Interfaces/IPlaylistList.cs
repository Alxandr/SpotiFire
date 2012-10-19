
namespace SpotiFire
{
    public interface IPlaylistList : IEditableArray<IContainerPlaylist>
    {
        IContainerPlaylist Add(string name);
    }
}

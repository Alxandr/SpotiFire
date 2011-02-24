using SpotiFire.SpotiClient.ServiceReference;

namespace SpotiFire.SpotiClient
{
    public class PlaylistTreeItem : Playlist
    {
        public PlaylistTreeItem(Playlist playlist, PlaylistTreeItem[] children = null)
        {
            if (children == null)
                children = new PlaylistTreeItem[0];
            Children = children;

            this.Description = playlist.Description;
            this.ExtensionData = playlist.ExtensionData;
            this.Id = playlist.Id;
            this.ImageId = playlist.ImageId;
            this.IsColaberativ = playlist.IsColaberativ;
            this.Name = playlist.Name;
            this.Type = playlist.Type;
        }
        public PlaylistTreeItem[] Children { get; private set; }
    }
}

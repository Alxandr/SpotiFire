using System;
namespace SpotiFire.SpotifyLib
{
    public interface IArtist : ISpotifyObject, IAsyncLoaded
    {
        bool IsLoaded { get; }
        string Name { get; }
        IArtistBrowse Browse(sp_artistbrowse_type type = sp_artistbrowse_type.FULL);
    }
}

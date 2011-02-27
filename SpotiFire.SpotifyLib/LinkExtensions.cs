
using System;
namespace SpotiFire.SpotifyLib
{
    public static class LinkExtensions
    {
        public static ILink<IArtist> CreateLink(this IArtist artist)
        {
            IntPtr linkPtr, artistPtr;
            artistPtr = Artist.GetPointer(artist);
            lock (libspotify.Mutex)
                linkPtr = libspotify.sp_link_create_from_artist(artistPtr);

            ILink<IArtist> link = (ILink<IArtist>)Link.Get((Session)artist.Session, linkPtr);
            lock (libspotify.Mutex)
                libspotify.sp_link_release(linkPtr);

            return link;
        }
    }
}

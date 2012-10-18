
using System;
namespace SpotiFire.SpotifyLib
{
    public static class LinkExtensions
    {
        public static ILink ParseLink(this ISession session, string link)
        {
            IntPtr linkPtr;
            lock (libspotify.Mutex)
                linkPtr = libspotify.sp_link_create_from_string(link);

            ILink lnk = Link.Get((Session)session, linkPtr);
            lock (libspotify.Mutex)
                libspotify.sp_link_release(linkPtr);

            return lnk;
        }

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

        public static ILink<ITrack> CreateLink(this ITrack track)
        {
            IntPtr linkPtr, trackPtr;
            trackPtr = Track.GetPointer(track);
            lock (libspotify.Mutex)
                linkPtr = libspotify.sp_link_create_from_track(trackPtr, 0);

            ILink<ITrack> link = (ILink<ITrack>)Link.Get((Session)track.Session, linkPtr);
            lock (libspotify.Mutex)
                libspotify.sp_link_release(linkPtr);

            return link;
        }

        public static T As<T>(this ILink link) where T : ISpotifyObject
        {
            switch (link.Type)
            {
                case LinkType.Track:
                    if (typeof(T) == typeof(ITrack)) return (T)((ILink<ITrackAndOffset>)link).Object.Track;
                    if (typeof(T) == typeof(ITrackAndOffset)) return (T)((ILink<ITrackAndOffset>)link).Object;
                    break;
                case LinkType.Album:
                    if (typeof(T) == typeof(IAlbum)) return (T)((ILink<IAlbum>)link).Object;
                    break;
                case LinkType.Artist:
                    if (typeof(T) == typeof(IArtist)) return (T)((ILink<IArtist>)link).Object;
                    break;
                case LinkType.Playlist:
                    if (typeof(T) == typeof(IPlaylist)) return (T)((ILink<IPlaylist>)link).Object;
                    break;
            }
            throw new ArgumentException("Invalid link");
        }
    }
}

using System;

namespace SpotiFire
{
    public static class LinkExtensions
    {
        public static Link ParseLink(this Session session, string link)
        {
            return Link.Create(session, link);
        }

        //public static Link<Artist> CreateLink(this Artist artist)
        //{
        //    IntPtr linkPtr, artistPtr;
        //    artistPtr = Types.Artist.GetPointer(artist);
        //    lock (libspotify.Mutex)
        //        linkPtr = SPLink.create_from_artist(artistPtr);

        //    Link<Artist> link = (Link<Artist>)Types.Link.Get((Types.Session)artist.Session, linkPtr);
        //    lock (libspotify.Mutex)
        //        SPLink.release(linkPtr);

        //    return link;
        //}

        //public static Link<Track> CreateLink(this Track track)
        //{
        //    IntPtr linkPtr, trackPtr;
        //    trackPtr = Types.Track.GetPointer(track);
        //    lock (libspotify.Mutex)
        //        linkPtr = SPLink.create_from_track(trackPtr, 0);

        //    Link<Track> link = (Link<Track>)Types.Link.Get((Types.Session)track.Session, linkPtr);
        //    lock (libspotify.Mutex)
        //        SPLink.release(linkPtr);

        //    return link;
        //}

        //public static T As<T>(this Link link) where T : ISpotifyObject
        //{
        //    switch (link.Type)
        //    {
        //        case LinkType.Track:
        //            if (typeof(T) == typeof(Track)) return (T)((Link<TrackAndOffset>)link).Object.Track;
        //            if (typeof(T) == typeof(TrackAndOffset)) return (T)((Link<TrackAndOffset>)link).Object;
        //            break;
        //        case LinkType.Album:
        //            if (typeof(T) == typeof(Album)) return (T)((Link<Album>)link).Object;
        //            break;
        //        case LinkType.Artist:
        //            if (typeof(T) == typeof(Artist)) return (T)((Link<Artist>)link).Object;
        //            break;
        //        case LinkType.Playlist:
        //            if (typeof(T) == typeof(Playlist)) return (T)((Link<Playlist>)link).Object;
        //            break;
        //    }
        //    throw new ArgumentException("Invalid link");
        //}
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using SPLink = SpotiFire.Session;

namespace SpotiFire.SpotifyLib
{
    internal abstract class Link : CountedDisposeableSpotifyObject, ILink
    {
        #region Wrapper
        private class LinkWrapper<T> : DisposeableSpotifyObject, ILink<T>
        {
            internal Link link;
            internal LinkWrapper(Link link)
            {
                this.link = link;
            }

            public T Object
            {
                get
                {
                    IsAlive(true);
                    return ((ILink<T>)link).Object;
                }
            }

            public LinkType Type
            {
                get
                {
                    IsAlive(true);
                    return link.Type;
                }
            }

            object ILink.Object
            {
                get { return Object; }
            }

            public ISession Session
            {
                get
                {
                    IsAlive(true);
                    return link.Session;
                }
            }

            protected override void OnDispose()
            {
                Link.Delete(link.linkPtr);
                link = null;
            }

            protected override int IntPtrHashCode()
            {
                return IsAlive() ? link.IntPtrHashCode() : 0;
            }

            public override string ToString()
            {
                return IsAlive() ? link.ToString() : String.Empty;
            }
        }
        internal static IntPtr GetPointer(ILink link)
        {
            if (typeof(LinkWrapper<>).IsInstanceOfType(link))
                return ((Link)typeof(LinkWrapper<>).GetField("link", BindingFlags.NonPublic).GetValue(link)).linkPtr;
            throw new ArgumentException("Invalid link");
        }
        #endregion
        #region Counter
        private static Dictionary<IntPtr, Link> links = new Dictionary<IntPtr, Link>();
        private static readonly object linksLock = new object();

        internal static ILink Get(Session session, IntPtr linkPtr)
        {
            Link link;
            lock (linksLock)
            {
                if (!links.ContainsKey(linkPtr))
                {
                    links.Add(linkPtr, Link.Create(session, linkPtr));
                }
                link = links[linkPtr];
                link.AddRef();
            }
            return Link.CreateWrapper(link);
        }

        internal static void Delete(IntPtr linkPtr)
        {
            lock (linksLock)
            {
                Link link = links[linkPtr];
                int count = link.RemRef();
                if (count == 0)
                    links.Remove(linkPtr);
            }
        }

        private static Link Create(Session session, IntPtr linkPtr)
        {
            LinkType type;
            lock (libspotify.Mutex)
                type = (LinkType)SPLink.link_type(linkPtr);

            switch (type)
            {
                case LinkType.Track:
                    return new TrackLink(session, linkPtr);
                case LinkType.Album:
                    return new AlbumLink(session, linkPtr);
                case LinkType.Artist:
                    return new ArtistLink(session, linkPtr);
                case LinkType.Search:
                    throw new ArgumentException("Search not supported.");
                case LinkType.Playlist:
                    return new PlaylistLink(session, linkPtr);
                case LinkType.Profile:
                    //x return new ProfileLink(session, linkPtr);
                    throw new NotImplementedException("User not implemented");
                case LinkType.Starred:
                    return new PlaylistLink(session, linkPtr);
                case LinkType.Localtrack:
                    return new TrackLink(session, linkPtr);
                default:
                    throw new ArgumentException("Invalid link.");
            }
        }

        private static ILink CreateWrapper(Link link)
        {
            var type = link.Type;
            switch (type)
            {
                case LinkType.Track:
                    return new LinkWrapper<ITrackAndOffset>(link);
                case LinkType.Album:
                    return new LinkWrapper<IAlbum>(link);
                case LinkType.Artist:
                    return new LinkWrapper<IArtist>(link);
                case LinkType.Search:
                    throw new ArgumentException("Search not supported.");
                case LinkType.Playlist:
                    return new LinkWrapper<IPlaylist>(link);
                case LinkType.Profile:
                    //x return new LinkWrapper<IUser>(link);
                    throw new NotImplementedException("User not implemented");
                case LinkType.Starred:
                    return new LinkWrapper<IPlaylist>(link);
                case LinkType.Localtrack:
                    return new LinkWrapper<ITrackAndOffset>(link);
                default:
                    throw new ArgumentException("Invalid link.");
            }
        }
        #endregion

        #region Declarations
        internal IntPtr linkPtr = IntPtr.Zero;
        protected Session session;
        #endregion

        #region Constructor
        private Link(Session session, IntPtr linkPtr)
        {
            if (linkPtr == IntPtr.Zero)
                throw new ArgumentException("linkPtr can't be zero.");

            if (session == null)
                throw new ArgumentNullException("Session can't be null.");
            this.session = session;
            this.linkPtr = linkPtr;

            lock (libspotify.Mutex)
                SPLink.link_add_ref(linkPtr);

            session.DisposeAll += new SessionEventHandler(session_DisposeAll);
        }

        void session_DisposeAll(ISession sender, SessionEventArgs e)
        {
            Dispose();
        }
        #endregion

        #region Properties
        public LinkType Type
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return (LinkType)SPLink.link_type(linkPtr);
            }
        }

        public ISession Session
        {
            get
            {
                IsAlive(true);
                return session;
            }
        }

        public abstract object Object { get; }
        #endregion

        #region Public Methods
        public override string ToString()
        {
            if (!IsAlive())
                return "";

            return SpotiFire.Session.link_as_string(linkPtr);
        }
        #endregion

        #region IDisposeable
        protected override void OnDispose()
        {
            session.DisposeAll -= new SessionEventHandler(session_DisposeAll);

            if (!session.ProcExit)
                lock (libspotify.Mutex)
                    SPLink.link_release(linkPtr);

            linkPtr = IntPtr.Zero;
        }
        #endregion

        #region LinkTypes
        private class ArtistLink : Link, ILink<IArtist>
        {
            public override object Object
            {
                get
                {
                    IsAlive(true);
                    lock (libspotify.Mutex)
                        return Artist.Get(session, SPLink.link_as_artist(linkPtr));
                }
            }

            internal ArtistLink(Session session, IntPtr linkPtr)
                : base(session, linkPtr) { }

            IArtist ILink<IArtist>.Object
            {
                get { return (IArtist)Object; }
            }

        }

        private class TrackLink : Link, ILink<ITrackAndOffset>
        {
            private class LinkAndOffset : ITrackAndOffset
            {
                private ITrack track;
                private TimeSpan offset;

                public LinkAndOffset(Session session, IntPtr linkPtr, int offset)
                {
                    this.offset = TimeSpan.FromSeconds(offset);
                    this.track = SpotifyLib.Track.Get(session, linkPtr);
                }

                public ITrack Track
                {
                    get { return track; }
                }

                public TimeSpan Offset
                {
                    get { return offset; }
                }
            }

            public override object Object
            {
                get
                {
                    IsAlive(true);
                    int offset;
                    IntPtr trackPtr;
                    lock (libspotify.Mutex)
                        trackPtr = SPLink.link_as_track_and_offset(linkPtr, out offset);
                    return new LinkAndOffset(session, trackPtr, offset);
                }
            }

            ITrackAndOffset ILink<ITrackAndOffset>.Object
            {
                get { return (ITrackAndOffset)Object; }
            }

            internal TrackLink(Session session, IntPtr linkPtr)
                : base(session, linkPtr) { }
        }

        private class AlbumLink : Link, ILink<IAlbum>
        {
            public override object Object
            {
                get
                {
                    IsAlive(true);
                    lock (libspotify.Mutex)
                        return Album.Get(session, SPLink.link_as_album(linkPtr));
                }
            }

            IAlbum ILink<IAlbum>.Object
            {
                get { return (IAlbum)Object; }
            }

            internal AlbumLink(Session session, IntPtr linkPtr)
                : base(session, linkPtr) { }
        }

        private class PlaylistLink : Link, ILink<IPlaylist>
        {
            public override object Object
            {
                get
                {
                    IsAlive(true);
                    lock (libspotify.Mutex)
                        return Playlist.Get(session, SpotiFire.Playlist.create(session.sessionPtr, linkPtr));
                }
            }

            IPlaylist ILink<IPlaylist>.Object
            {
                get { return (IPlaylist)Object; }
            }

            internal PlaylistLink(Session session, IntPtr linkPtr)
                : base(session, linkPtr) { }
        }
        //TODO: User link
        #endregion

        protected override int IntPtrHashCode()
        {
            return linkPtr.GetHashCode();
        }
    }
}

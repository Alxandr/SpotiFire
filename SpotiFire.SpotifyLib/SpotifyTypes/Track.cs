using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SPTrack = SpotiFire.Track;

namespace SpotiFire.Types
{
    internal class Track : CountedDisposeableSpotifyObject, ITrack
    {
        #region Wrapper
        protected class TrackWrapper : DisposeableSpotifyObject, ITrack
        {
            internal Track track;
            internal TrackWrapper(Track track)
            {
                this.track = track;
            }

            protected override void OnDispose()
            {
                Track.Delete(track.trackPtr);
                track = null;
            }

            public IAlbum Album
            {
                get { IsAlive(true); return track.Album; }
            }

            public IArray<IArtist> Artists
            {
                get { IsAlive(true); return track.Artists; }
            }

            public int Disc
            {
                get { IsAlive(true); return track.Disc; }
            }

            public TimeSpan Duration
            {
                get { IsAlive(true); return track.Duration; }
            }

            public Error Error
            {
                get { IsAlive(true); return track.Error; }
            }

            public int Index
            {
                get { IsAlive(true); return track.Index; }
            }

            public TrackAvailability Availability
            {
                get { IsAlive(true); return track.Availability; }
            }

            public bool IsLoaded
            {
                get { IsAlive(true); return track.IsLoaded; }
            }

            public bool IsReady
            {
                get { IsAlive(true); return track.IsReady; }
            }

            public string Name
            {
                get { IsAlive(true); return track.Name; }
            }

            public int Popularity
            {
                get { IsAlive(true); return track.Popularity; }
            }

            public bool IsStarred
            {
                get { IsAlive(true); return track.IsStarred; }
                set { IsAlive(true); track.IsStarred = value; }
            }

            public ISession Session
            {
                get { IsAlive(true); return track.Session; }
            }

            protected override int IntPtrHashCode()
            {
                return IsAlive() ? track.trackPtr.GetHashCode() : 0;
            }
        }
        internal static IntPtr GetPointer(ITrack track)
        {
            if (track is TrackWrapper)
                return ((TrackWrapper)track).track.trackPtr;
            throw new ArgumentException("Invalid track");
        }
        #endregion
        #region Counter
        private static Dictionary<IntPtr, Track> tracks = new Dictionary<IntPtr, Track>();
        private static readonly object tracksLock = new object();

        internal static ITrack Get(Session session, IntPtr trackPtr)
        {
            Track track;
            lock (tracksLock)
            {
                if (!tracks.ContainsKey(trackPtr))
                {
                    tracks.Add(trackPtr, new Track(session, trackPtr));
                }
                track = tracks[trackPtr];
                track.AddRef();
            }
            return new TrackWrapper(track);
        }

        internal static void Delete(IntPtr trackPtr)
        {
            lock (tracksLock)
            {
                Track track = tracks[trackPtr];
                int count = track.RemRef();
                if (count == 0)
                    tracks.Remove(trackPtr);
            }
        }
        #endregion

        #region Declarations
        internal IntPtr trackPtr = IntPtr.Zero;
        private Session session;

        private IAlbum album = null;
        private DelegateArray<IArtist> artists = null;
        #endregion

        #region Constructor
        internal protected Track(Session session, IntPtr trackPtr)
        {
            if (trackPtr == IntPtr.Zero)
                throw new ArgumentException("trackPtr can't be zero.");

            if (session == null)
                throw new ArgumentNullException("Session can't be null.");
            this.session = session;
            this.trackPtr = trackPtr;
            lock (libspotify.Mutex)
                SPTrack.add_ref(trackPtr);

            this.artists = new DelegateArray<IArtist>(() =>
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return SPTrack.num_artists(trackPtr);
            }, (index) =>
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return Artist.Get(session, SPTrack.artist(trackPtr, index));
            });

            session.DisposeAll += new SessionEventHandler(session_DisposeAll);
        }

        void session_DisposeAll(ISession sender, SessionEventArgs e)
        {
            Dispose();
        }
        #endregion

        #region Properties
        public bool IsLoaded
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return SPTrack.is_loaded(trackPtr);
            }
        }

        public TrackAvailability Availability
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return (TrackAvailability)SPTrack.get_availability(session.sessionPtr, trackPtr);
            }
        }

        public Error Error
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return (Error)SPTrack.error(trackPtr);
            }
        }

        public IAlbum Album
        {
            get
            {
                IsAlive(true);
                if (album == null)
                    lock (libspotify.Mutex)
                        album = Types.Album.Get(session, SPTrack.album(trackPtr));

                return album;
            }
        }

        public IArray<IArtist> Artists
        {
            get
            {
                IsAlive(true);
                return artists;
            }
        }

        public string Name
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return SPTrack.name(trackPtr);
            }
        }

        public TimeSpan Duration
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return TimeSpan.FromMilliseconds(SPTrack.duration(trackPtr));
            }
        }

        public int Popularity
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return SPTrack.popularity(trackPtr);
            }
        }

        public bool IsStarred
        {
            get
            {
                IsAlive(true);

                //lock (libspotify.Mutex)
                //    libspotify.sp_track_is_starred(session.sessionPtr, trackPtr);

                return SpotiFire.Track.is_starred(session.sessionPtr, trackPtr);

                //return session.Starred.Tracks.Any(t => Track.GetPointer(t) == this.trackPtr);
            }
            set
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    SPTrack.set_starred(session.sessionPtr, new[] { trackPtr }, value);
            }
        }

        public int Disc
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return SPTrack.disc(trackPtr);
            }
        }

        public int Index
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return SPTrack.index(trackPtr);
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
        #endregion

        #region IDisposeable
        protected override void OnDispose()
        {
            session.DisposeAll -= new SessionEventHandler(session_DisposeAll);

            if (!session.ProcExit)
                lock (libspotify.Mutex)
                    SPTrack.release(trackPtr);

            trackPtr = IntPtr.Zero;
        }
        #endregion

        #region Await
        public bool IsReady
        {
            get { return !string.IsNullOrEmpty(Name); }
        }
        #endregion

        protected override int IntPtrHashCode()
        {
            return trackPtr.GetHashCode();
        }
    }
}

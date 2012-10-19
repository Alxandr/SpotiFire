﻿using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.Linq.Expressions;
using SPAlbumbrowse = SpotiFire.Albumbrowse;

namespace SpotiFire
{
    public delegate void AlbumBrowseEventHandler(IAlbumBrowse sender, AlbumBrowseEventArgs e);
}

namespace SpotiFire.Types
{
    internal class AlbumBrowse : CountedDisposeableSpotifyObject, IAlbumBrowse, ISpotifyAwaitable {

        #region Wrapper
        private sealed class AlbumBrowseWrapper : DisposeableSpotifyObject, IAlbumBrowse, ISpotifyAwaitable {
            internal AlbumBrowse albumBrowse;
            public AlbumBrowseWrapper(AlbumBrowse albumBrowse) {
                this.albumBrowse = albumBrowse;
                albumBrowse.Complete += new AlbumBrowseEventHandler(albumBrowse_Complete);
            }

            void albumBrowse_Complete(IAlbumBrowse sender, AlbumBrowseEventArgs e) {
                if (sender == albumBrowse)
                    if (Complete != null)
                        Complete(this, e);
            }

            public event AlbumBrowseEventHandler Complete;

            protected override void OnDispose() {
                albumBrowse.Complete -= new AlbumBrowseEventHandler(albumBrowse_Complete);
                AlbumBrowse.Delete(albumBrowse.albumBrowsePtr);
                albumBrowse = null;
            }

            public Error Error {
                get { IsAlive(true); return albumBrowse.Error; }
            }

            public IAlbum Album {
                get { IsAlive(true); return albumBrowse.Album; }
            }

            public IArtist Artist {
                get { IsAlive(true); return albumBrowse.Artist; }
            }

            public IArray<string> Copyrights {
                get { IsAlive(true); return albumBrowse.Copyrights; }
            }

            public IArray<ITrack> Tracks {
                get { IsAlive(true); return albumBrowse.Tracks; }
            }

            public string Review {
                get { IsAlive(true); return albumBrowse.Review; }
            }

            public ISession Session {
                get { IsAlive(true); return albumBrowse.Session; }
            }

            public bool IsComplete {
                get { IsAlive(true); return albumBrowse.IsComplete; }
            }

            protected override int IntPtrHashCode() {
                return IsAlive() ? albumBrowse.albumBrowsePtr.GetHashCode() : 0;
            }


            bool ISpotifyAwaitable.IsComplete
            {
                get { IsAlive(true); return ((ISpotifyAwaitable)albumBrowse).IsComplete; }
            }

            bool ISpotifyAwaitable.AddContinuation(AwaitHelper.Continuation continuation, bool addBeforeOthers)
            {
                IsAlive(true); return ((ISpotifyAwaitable)albumBrowse).AddContinuation(continuation, addBeforeOthers);
            }
        }
        internal static IntPtr GetPointer(IAlbumBrowse albumBrowse) {
            if (albumBrowse.GetType() == typeof(AlbumBrowseWrapper))
                return ((AlbumBrowseWrapper)albumBrowse).albumBrowse.albumBrowsePtr;
            throw new ArgumentException("Invalid album browse");
        }
        #endregion
        #region Counter
        private static Dictionary<IntPtr, AlbumBrowse> albumBrowsers = new Dictionary<IntPtr, AlbumBrowse>();
        private static readonly object albumsBrowseLock = new object();

        internal static IAlbumBrowse Get(Session session, IntPtr albumBrowsePtr) {
            AlbumBrowse albumBrowse;
            lock (albumsBrowseLock) {
                if (!albumBrowsers.ContainsKey(albumBrowsePtr)) {
                    albumBrowsers.Add(albumBrowsePtr, new AlbumBrowse(session, albumBrowsePtr));
                }
                albumBrowse = albumBrowsers[albumBrowsePtr];
                albumBrowse.AddRef();
            }
            return new AlbumBrowseWrapper(albumBrowse);
        }

        internal static void Delete(IntPtr albumBrowsePtr) {
            lock (albumsBrowseLock) {
                AlbumBrowse albumBrowse = albumBrowsers[albumBrowsePtr];
                int count = albumBrowse.RemRef();
                if (count == 0)
                    albumBrowsers.Remove(albumBrowsePtr);
            }
        }
        #endregion

        #region Delegates
        internal delegate void albumbrowse_complete_cb(IntPtr albumBrowsePtr, IntPtr userdataPtr);
        #endregion

        #region Declarations

        internal IntPtr albumBrowsePtr = IntPtr.Zero;
        private Session session;
        private bool isComplete = false;

        internal static albumbrowse_complete_cb albumbrowse_complete = new albumbrowse_complete_cb(_AlbumBrowseCompleteCallback);

        private IAlbum album;
        private IArtist artist;
        private DelegateArray<string> copyrights;
        private DelegateArray<ITrack> tracks;
        private string review;
        #endregion

        #region Constructor and setup
        private AlbumBrowse(Session session, IntPtr albumBrowsePtr) {

            if (albumBrowsePtr == IntPtr.Zero)
                throw new ArgumentException("albumBrowsePtr can't be zero.");

            if (session == null)
                throw new ArgumentNullException("Session can't be null.");
            this.session = session;

            this.albumBrowsePtr = albumBrowsePtr;

            this.copyrights = new DelegateArray<string>(() => {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return SPAlbumbrowse.num_copyrights(albumBrowsePtr);
            }, (index) => {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return SPAlbumbrowse.copyright(albumBrowsePtr, index);
            });

            this.tracks = new DelegateArray<ITrack>(() => {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return SPAlbumbrowse.num_tracks(albumBrowsePtr);
            }, (index) => {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return Track.Get(session, SPAlbumbrowse.track(albumBrowsePtr, index));
            });

            _Complete += new albumbrowse_complete_cb(AlbumBrowse__Complete);
            session.DisposeAll += new SessionEventHandler(session_DisposeAll);
        }

        private void AlbumBrowse__Complete(IntPtr albumBrowsePtr, IntPtr userdataPtr) {
            if (albumBrowsePtr == this.albumBrowsePtr) {
                lock (this)
                    this.isComplete = true;
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<AlbumBrowseEventArgs>(ab => ab.OnComplete, this), new AlbumBrowseEventArgs(this)));
                _Complete -= new albumbrowse_complete_cb(AlbumBrowse__Complete);
            }
        }

        static void _AlbumBrowseCompleteCallback(IntPtr albumBrowsePtr, IntPtr userdataPtr) {
            if (_Complete != null)
                _Complete(albumBrowsePtr, userdataPtr);
        }
        #endregion

        #region Properties

        public Error Error {
            get {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return (Error)SPAlbumbrowse.error(albumBrowsePtr);
            }
        }

        public IAlbum Album {
            get {
                if (album == null) {
                    IsAlive(true);
                    lock (libspotify.Mutex)
                        album = Types.Album.Get(session, SPAlbumbrowse.album(albumBrowsePtr));
                }
                return album;
            }
        }

        public IArtist Artist {
            get {
                if (album == null) {
                    IsAlive(true);
                    lock (libspotify.Mutex)
                        artist = Types.Artist.Get(session, SPAlbumbrowse.artist(albumBrowsePtr));
                }
                return artist;
            }
        }

        public IArray<string> Copyrights {
            get {
                return copyrights;
            }
        }

        public IArray<ITrack> Tracks {
            get {
                return tracks;
            }
        }

        public string Review {
            get {
                if (review == null) {
                    IsAlive(true);
                    lock (libspotify.Mutex)
                        review = SPAlbumbrowse.review(albumBrowsePtr);
                }
                return review;
            }
        }

        public ISession Session {
            get {
                IsAlive(true);
                return session;
            }
        }

        public bool IsComplete {
            get {
                IsAlive(true);
                return isComplete;
            }
        }
        #endregion

        #region Public methods

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[AlbumBrowse]");
            sb.AppendLine("Error=" + Error);
            sb.AppendLine(Album.ToString());
            sb.AppendLine(Artist.ToString());
            sb.AppendLine("Copyrights=" + string.Join(",", Copyrights));
            sb.AppendLine("Tracks.Length=" + Tracks.Count);
            foreach (Track t in Tracks)
                sb.AppendLine(t.ToString());

            sb.AppendFormat("Review:{0}{1}", Environment.NewLine, Review);

            return sb.ToString();
        }


        #endregion

        #region Cleanup

        protected override void OnDispose() {
            _Complete -= new albumbrowse_complete_cb(AlbumBrowse__Complete);
            session.DisposeAll -= new SessionEventHandler(session_DisposeAll);

            if (!session.ProcExit)
                lock (libspotify.Mutex)
                    SPAlbumbrowse.release(albumBrowsePtr);

            albumBrowsePtr = IntPtr.Zero;
        }

        private void session_DisposeAll(ISession sender, SessionEventArgs e) {
            Dispose();
        }
        #endregion

        #region Private methods
        private static Delegate CreateDelegate<T>(Expression<Func<AlbumBrowse, Action<T>>> expr, AlbumBrowse ab) where T : AlbumBrowseEventArgs {
            return expr.Compile().Invoke(ab);
        }
        #endregion

        #region Protected Methods
        protected virtual void OnComplete(AlbumBrowseEventArgs args) {
            if (this.Complete != null)
                this.Complete(this, args);
        }
        #endregion

        #region Event
        public event AlbumBrowseEventHandler Complete;
        private static event albumbrowse_complete_cb _Complete;
        #endregion

        #region Await
        bool ISpotifyAwaitable.IsComplete
        {
            get
            {
                lock (this)
                    return IsComplete;
            }
        }

        AlbumBrowseEventHandler _handler;
        List<AwaitHelper.Continuation> _handlers;
        private void EnsureHandler()
        {
            if (_handlers == null)
            {
                _handlers = new List<AwaitHelper.Continuation>();
                _handler = (s, e) =>
                {
                    lock (this)
                    {
                        foreach (var h in _handlers)
                            h.Run(this, true);
                        _handlers = null;
                        Complete -= _handler;
                        _handler = null;
                    }
                };
                Complete += _handler;
            }
        }

        bool ISpotifyAwaitable.AddContinuation(AwaitHelper.Continuation continuation, bool addBeforeOthers)
        {
            lock (this)
            {
                if (IsComplete)
                {
                    return false;
                }
                else
                {
                    EnsureHandler();
                    if (addBeforeOthers)
                        _handlers.Insert(0, continuation);
                    else
                        _handlers.Add(continuation);
                    return true;
                }
            }
        }
        #endregion

        protected override int IntPtrHashCode() {
            throw new NotImplementedException();
        }

    }
}

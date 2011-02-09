using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace SpotiFire.SpotifyLib
{
    public delegate void PlaylistEventHandler(IPlaylist playlist, EventArgs args);
    public delegate void PlaylistEventHandler<TEventArgs>(IPlaylist playlist, TEventArgs args) where TEventArgs : EventArgs;



    internal class Playlist : CountedDisposeableSpotifyObject, IPlaylist
    {
        #region KeyGen
        private class PlaylistMaintainer : Tuple<IntPtr, sp_playlist_type, IntPtr>
        {
            public PlaylistMaintainer(IntPtr ptr, sp_playlist_type type, IntPtr id)
                : base(ptr, type, id)
            {
            }
        }
        #endregion
        #region Wrapper
        private class PlaylistWrapper : DisposeableSpotifyObject, IPlaylist
        {
            internal Playlist playlist;
            public PlaylistWrapper(Playlist playlist)
            {
                this.playlist = playlist;
            }

            protected override void OnDispose()
            {
                Playlist.Delete(playlist.playlistPtr, playlist.type, playlist.folderId);
                playlist = null;
            }

            protected override int IntPtrHashCode()
            {
                return IsAlive() ? playlist.IntPtrHashCode() : 0;
            }

            public IEditableArray<ITrack> Tracks
            {
                get { IsAlive(true); return playlist.Tracks; }
            }

            public sp_playlist_type Type
            {
                get { IsAlive(true); return playlist.Type; }
            }

            public string Name
            {
                get { IsAlive(true); return playlist.Name; }
                set { IsAlive(true); playlist.Name = value; }
            }

            public Session Session
            {
                get { IsAlive(true); return playlist.Session; }
            }

            public override bool Equals(object obj)
            {
                PlaylistWrapper pw = obj as PlaylistWrapper;
                if (pw == null)
                {
                    Playlist p = obj as Playlist;
                    if (p == null)
                        return false;
                    return p == this.playlist;
                }
                return pw.playlist == this.playlist;
            }
        }
        internal static IntPtr GetPointer(IPlaylist playlist)
        {
            if (playlist.GetType() == typeof(PlaylistWrapper))
                return ((PlaylistWrapper)playlist).playlist.playlistPtr;
            throw new ArgumentException("Invalid playlist");
        }
        #endregion
        #region Counter
        private static Dictionary<PlaylistMaintainer, Playlist> playlists = new Dictionary<PlaylistMaintainer, Playlist>();
        private static readonly object playlistsLock = new object();

        internal static IPlaylist Get(Session session, PlaylistContainer container, IntPtr playlistPtr, sp_playlist_type type, IntPtr id)
        {
            Playlist playlist;
            PlaylistMaintainer pm = new PlaylistMaintainer(playlistPtr, type, id);
            lock (playlistsLock)
            {
                if (!playlists.ContainsKey(pm))
                {
                    playlists.Add(pm, new Playlist(session, container, playlistPtr, type, id));
                }
                playlist = playlists[pm];
                playlist.AddRef();
            }
            return new PlaylistWrapper(playlist);
        }

        internal static void Delete(IntPtr playlistPtr, sp_playlist_type type, IntPtr id)
        {
            PlaylistMaintainer pm = new PlaylistMaintainer(playlistPtr, type, id);
            lock (playlistsLock)
            {
                Playlist playlist = playlists[pm];
                int count = playlist.RemRef();
                if (count == 0)
                    playlists.Remove(pm);
            }
        }
        #endregion

        #region Spotify Delegates
        #region Struct
        private struct sp_playlist_callbacks
        {
            internal IntPtr tracks_added;
            internal IntPtr tracks_removed;
            internal IntPtr tracks_moved;
            internal IntPtr playlist_renamed;
            internal IntPtr playlist_state_changed;
            internal IntPtr playlist_update_in_progress;
            internal IntPtr playlist_metadata_updated;
            internal IntPtr track_created_changed;
            internal IntPtr track_seen_changed;
            internal IntPtr description_changed;
            internal IntPtr image_changed;
        }
        #endregion
        private delegate void tracks_added_cb(IntPtr playlistPtr, IntPtr tracksPtr, int num_tracks, int position, IntPtr userdataPtr);
        private delegate void tracks_removed_cb(IntPtr playlistPtr, IntPtr trackIndicesPtr, int num_tracks, IntPtr userdataPtr);
        private delegate void tracks_moved_cd(IntPtr playlistPtr, IntPtr trackIndicesPtr, int num_tracks, int new_position, IntPtr userdataPtr);
        private delegate void playlist_renamed_cb(IntPtr playlistPtr, IntPtr userdataPtr);
        private delegate void playlist_state_changed_cb(IntPtr playlistPtr, IntPtr userdataPtr);
        private delegate void playlist_update_in_progress_cb(IntPtr playlistPtr, bool done, IntPtr userdataPtr);
        private delegate void playlist_metadata_updated_cb(IntPtr playlistPtr, IntPtr userdataPtr);
        private delegate void track_created_changed_cb(IntPtr playlistPtr, int position, IntPtr userPtr, int when, IntPtr userdataPtr);
        private delegate void track_seen_changed_cb(IntPtr playlistPtr, int position, bool seen, IntPtr userdataPtr);
        private delegate void description_changed_cb(IntPtr playlistPtr, IntPtr descPtr, IntPtr userdataPtr);
        private delegate void image_changed_cb(IntPtr playlistPtr, IntPtr imgIdPtr, IntPtr userdataPtr);
        #endregion

        #region Spotify Callbacks
        private tracks_added_cb tracks_added;
        private tracks_removed_cb tracks_removed;
        private tracks_moved_cd tracks_moved;
        private playlist_renamed_cb playlist_renamed;
        private playlist_state_changed_cb playlist_state_changed;
        private playlist_update_in_progress_cb playlist_update_in_progress;
        private playlist_metadata_updated_cb playlist_metadata_updated;
        private track_created_changed_cb track_created_changed;
        private track_seen_changed_cb track_seen_changed;
        private description_changed_cb description_changed;
        private image_changed_cb image_changed;
        #endregion

        #region Spotify Callback Implementations
        private void TracksAddedCallback(IntPtr playlistPtr, IntPtr tracksPtr, int num_tracks, int position, IntPtr userdataPtr)
        {
            if (playlistPtr == this.playlistPtr)
            {
                int[] trackIndices = new int[num_tracks];
                ITrack[] tracks = new ITrack[num_tracks];
                IntPtr[] trackPtrs = new IntPtr[num_tracks];
                Marshal.Copy(tracksPtr, trackPtrs, 0, num_tracks);

                for (int i = 0; i < num_tracks; i++)
                {
                    trackIndices[i] = position + i;
                    tracks[i] = Track.Get(session, trackPtrs[i]);
                }

                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<TracksAddedEventArgs>(p => p.OnTracksAdded, this), new TracksAddedEventArgs(trackIndices, tracks)));
            }
        }

        private void TracksRemovedCallback(IntPtr playlistPtr, IntPtr trackIndicesPtr, int num_tracks, IntPtr userdataPtr)
        {
            if (playlistPtr == this.playlistPtr)
            {
                int[] trackIndices = new int[num_tracks];
                Marshal.Copy(trackIndicesPtr, trackIndices, 0, num_tracks);
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<TracksEventArgs>(p => p.OnTracksRemoved, this), new TracksEventArgs(trackIndices)));
            }
        }

        private void TracksMovedCallback(IntPtr playlistPtr, IntPtr trackIndicesPtr, int num_tracks, int new_position, IntPtr userdataPtr)
        {
            if (playlistPtr == this.playlistPtr)
            {
                int[] trackIndices = new int[num_tracks];
                Marshal.Copy(trackIndicesPtr, trackIndices, 0, num_tracks);
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<TracksMovedEventArgs>(p => p.OnTracksMoved, this), new TracksMovedEventArgs(trackIndices, new_position)));
            }
        }

        private void RenamedCallback(IntPtr playlistPtr, IntPtr userdataPtr)
        {
            if (playlistPtr == this.playlistPtr)
            {
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<EventArgs>(p => p.OnRenamed, this), new EventArgs()));
            }
        }

        private void StateChangedCallback(IntPtr playlistPtr, IntPtr userdataPtr)
        {
            if (playlistPtr == this.playlistPtr)
            {
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<EventArgs>(p => p.OnStateChanged, this), new EventArgs()));
            }
        }

        private void UdateInProgressCallback(IntPtr playlistPtr, bool complete, IntPtr userdataPtr)
        {
            if (playlistPtr == this.playlistPtr)
            {
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<PlaylistUpdateEventArgs>(p => p.OnUpdateInProgress, this), new PlaylistUpdateEventArgs(complete)));
            }
        }

        private void MetadataUpdatedCallback(IntPtr playlistPtr, IntPtr userdataPtr)
        {
            if (playlistPtr == this.playlistPtr)
            {
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<EventArgs>(p => p.OnMetadataUpdated, this), new EventArgs()));
            }
        }

        private void TrackCreatedChangedCallback(IntPtr playlistPtr, int position, IntPtr userPtr, int when, IntPtr userdataPtr)
        {
            if (playlistPtr == this.playlistPtr)
            {
                ITrack track = Track.Get(session, libspotify.sp_playlist_track(playlistPtr, position));
                //IUser user = User.Get(session, userPtr);
                DateTime dtWhen = new DateTime(TimeSpan.FromSeconds(when).Ticks, DateTimeKind.Utc);
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<TrackCreatedChangedEventArgs>(p => p.OnTrackCreatedChanged, this), new TrackCreatedChangedEventArgs(track, dtWhen)));
            }
        }

        private void TrackSeenChangedCallback(IntPtr playlistPtr, int position, bool seen, IntPtr userdataPtr)
        {
            if (playlistPtr == this.playlistPtr)
            {
                ITrack track = Track.Get(session, libspotify.sp_playlist_track(playlistPtr, position));
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<TrackSeenEventArgs>(p => p.OnTrackSeenChanged, this), new TrackSeenEventArgs(track, seen)));
            }
        }

        private void DescriptionChangedCallback(IntPtr playlistPtr, IntPtr descriptionPtr, IntPtr userdataPtr)
        {
            if (playlistPtr == this.playlistPtr)
            {
                string description = libspotify.GetString(descriptionPtr, String.Empty);
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<DescriptionEventArgs>(p => p.OnDescriptionChanged, this), new DescriptionEventArgs(description)));
            }
        }

        private void ImageChangedCallback(IntPtr playlistPtr, IntPtr imageIdPtr, IntPtr userdataPtr)
        {
            if (playlistPtr == this.playlistPtr)
            {
                string imgId = libspotify.ImageIdToString(imageIdPtr);
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<ImageEventArgs>(p => p.OnImageChanged, this), new ImageEventArgs(imgId)));
            }
        }
        #endregion

        #region Event Functions
        protected virtual void OnTracksAdded(TracksAddedEventArgs args)
        {
            if (TracksAdded != null)
                TracksAdded(this, args);
        }
        protected virtual void OnTracksRemoved(TracksEventArgs args)
        {
            if (TracksRemoved != null)
                TracksRemoved(this, args);
        }
        protected virtual void OnTracksMoved(TracksMovedEventArgs args)
        {
            if (TracksMoved != null)
                TracksMoved(this, args);
        }
        protected virtual void OnRenamed(EventArgs args)
        {
            if (Renamed != null)
                Renamed(this, args);
        }
        protected virtual void OnStateChanged(EventArgs args)
        {
            if (StateChanged != null)
                StateChanged(this, args);
        }
        protected virtual void OnUpdateInProgress(PlaylistUpdateEventArgs args)
        {
            if (UpdateInProgress != null)
                UpdateInProgress(this, args);
        }
        protected virtual void OnMetadataUpdated(EventArgs args)
        {
            if (MetadataUpdated != null)
                MetadataUpdated(this, args);
        }
        protected virtual void OnTrackCreatedChanged(TrackCreatedChangedEventArgs args)
        {
            if (TrackCreatedChanged != null)
                TrackCreatedChanged(this, args);
        }
        protected virtual void OnTrackSeenChanged(TrackSeenEventArgs args)
        {
            if (TrackSeenChanged != null)
                TrackSeenChanged(this, args);
        }
        protected virtual void OnDescriptionChanged(DescriptionEventArgs args)
        {
            if (DescriptionChanged != null)
                DescriptionChanged(this, args);
        }
        protected virtual void OnImageChanged(ImageEventArgs args)
        {
            if (ImageChanged != null)
                ImageChanged(this, args);
        }
        #endregion

        #region Events
        public event PlaylistEventHandler<TracksAddedEventArgs> TracksAdded;
        public event PlaylistEventHandler<TracksEventArgs> TracksRemoved;
        public event PlaylistEventHandler<TracksMovedEventArgs> TracksMoved;
        public event PlaylistEventHandler Renamed;
        public event PlaylistEventHandler StateChanged;
        public event PlaylistEventHandler<PlaylistUpdateEventArgs> UpdateInProgress;
        public event PlaylistEventHandler MetadataUpdated;
        public event PlaylistEventHandler<TrackCreatedChangedEventArgs> TrackCreatedChanged;
        public event PlaylistEventHandler<TrackSeenEventArgs> TrackSeenChanged;
        public event PlaylistEventHandler<DescriptionEventArgs> DescriptionChanged;
        public event PlaylistEventHandler<ImageEventArgs> ImageChanged;
        #endregion

        #region Declarations
        internal IntPtr playlistPtr = IntPtr.Zero;
        private Session session;
        private PlaylistContainer container;
        private sp_playlist_callbacks callbacks;
        private IntPtr callbacksPtr = IntPtr.Zero;
        private sp_playlist_type type;
        private IntPtr folderId = IntPtr.Zero;

        private IEditableArray<ITrack> tracks;
        #endregion

        #region Constructor
        private Playlist(Session session, PlaylistContainer container, IntPtr playlistPtr, sp_playlist_type type, IntPtr folderId)
        {
            if (playlistPtr == IntPtr.Zero)
                throw new ArgumentException("playlistPtr can't be zero.");

            if (session == null)
                throw new ArgumentNullException("Session can't be null.");

            if (container == null)
                throw new ArgumentNullException("Container can't be null.");
            this.session = session;
            this.container = container;
            this.playlistPtr = playlistPtr;
            this.type = type;
            this.folderId = folderId;
            lock (libspotify.Mutex)
            {
                libspotify.sp_playlist_add_ref(playlistPtr);
                // TODO: Implement listeners
            }

            session.DisposeAll += new SessionEventHandler(session_DisposeAll);

            tracks = new DelegateList<ITrack>(
                () =>
                {
                    IsAlive(true);
                    lock (libspotify.Mutex)
                        return libspotify.sp_playlist_num_tracks(playlistPtr);
                },
                (index) =>
                {
                    IsAlive(true);
                    lock (libspotify.Mutex)
                        return Track.Get(session, libspotify.sp_playlist_track(playlistPtr, index));
                },
                (track, index) =>
                {
                    IntPtr[] ptrArray = new IntPtr[1];
                    IntPtr trackArrayPtr = Marshal.AllocHGlobal(Marshal.SizeOf(ptrArray));
                    Marshal.Copy(ptrArray, 0, trackArrayPtr, 1);
                    IsAlive(true);
                    lock (libspotify.Mutex)
                        libspotify.sp_playlist_add_tracks(playlistPtr, trackArrayPtr, 1, index, session.sessionPtr);
                    Marshal.FreeHGlobal(trackArrayPtr);
                },
                (index) =>
                {
                    IsAlive(true);
                    lock (libspotify.Mutex)
                        libspotify.sp_playlist_remove_tracks(playlistPtr, new int[] { index }, 1);
                },
                () => false
            );
        }

        void session_DisposeAll(Session sender, SessionEventArgs e)
        {
            Dispose();
        }
        #endregion

        #region Private Methods
        private static Delegate CreateDelegate<T>(Expression<Func<Playlist, Action<T>>> expr, Playlist p) where T : EventArgs
        {
            return expr.Compile().Invoke(p);
        }
        #endregion

        #region Properties
        public string Name
        {
            get
            {
                IsAlive(true);
                if (type == sp_playlist_type.SP_PLAYLIST_TYPE_PLAYLIST)
                    lock (libspotify.Mutex)
                        return libspotify.GetString(libspotify.sp_playlist_name(playlistPtr), String.Empty);
                else if (type != sp_playlist_type.SP_PLAYLIST_TYPE_PLACEHOLDER)
                    return container.GetFolderName(this);
                throw new InvalidOperationException();
            }
            set
            {
                if (value.Length > 255)
                    throw new ArgumentException("value can't be longer than 255 chars.");
                IsAlive(true);
                lock (libspotify.Mutex)
                    libspotify.sp_playlist_rename(playlistPtr, value);
            }
        }

        public IEditableArray<ITrack> Tracks
        {
            get
            {
                IsAlive(true);
                return tracks;
            }
        }

        public sp_playlist_type Type
        {
            get
            {
                IsAlive(true);
                return type;
            }
        }

        public Session Session
        {
            get { IsAlive(true); return session; }
        }
        #endregion

        #region Cleanup
        protected override void OnDispose()
        {
            session.DisposeAll -= new SessionEventHandler(session_DisposeAll);

            if (!session.ProcExit)
                lock (libspotify.Mutex)
                {
                    libspotify.sp_playlist_release(playlistPtr);
                }

            playlistPtr = IntPtr.Zero;
        }
        #endregion

        protected override int IntPtrHashCode()
        {
            return playlistPtr.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return false;
            if (obj.GetType() == typeof(Playlist))
            {
                Playlist p = (Playlist)obj;
                return p.playlistPtr == this.playlistPtr && p.type == this.type && p.folderId == this.folderId;
            }
            return false;
        }
    }
}

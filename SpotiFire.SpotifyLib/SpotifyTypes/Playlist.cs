using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using SPPlaylist = SpotiFire.Playlist;

namespace SpotiFire
{
    public delegate void PlaylistEventHandler(IPlaylist playlist, EventArgs args);
    public delegate void PlaylistEventHandler<TEventArgs>(IPlaylist playlist, TEventArgs args) where TEventArgs : EventArgs;
}

namespace SpotiFire.Types
{
    internal class Playlist : CountedDisposeableSpotifyObject, IPlaylist
    {
        #region Wrapper
        internal protected class PlaylistWrapper : DisposeableSpotifyObject, IPlaylist
        {
            internal Playlist playlist;
            public PlaylistWrapper(Playlist playlist)
            {
                this.playlist = playlist;
                playlist.TracksAdded += new PlaylistEventHandler<TracksAddedEventArgs>(playlist_TracksAdded);
                playlist.TracksRemoved += new PlaylistEventHandler<TracksEventArgs>(playlist_TracksRemoved);
                playlist.TracksMoved += new PlaylistEventHandler<TracksMovedEventArgs>(playlist_TracksMoved);
                playlist.Renamed += new PlaylistEventHandler(playlist_Renamed);
                playlist.StateChanged += new PlaylistEventHandler(playlist_StateChanged);
                playlist.UpdateInProgress += new PlaylistEventHandler<PlaylistUpdateEventArgs>(playlist_UpdateInProgress);
                playlist.MetadataUpdated += new PlaylistEventHandler(playlist_MetadataUpdated);
                playlist.TrackCreatedChanged += new PlaylistEventHandler<TrackCreatedChangedEventArgs>(playlist_TrackCreatedChanged);
                playlist.TrackSeenChanged += new PlaylistEventHandler<TrackSeenEventArgs>(playlist_TrackSeenChanged);
                playlist.DescriptionChanged += new PlaylistEventHandler<DescriptionEventArgs>(playlist_DescriptionChanged);
                playlist.ImageChanged += new PlaylistEventHandler<ImageEventArgs>(playlist_ImageChanged);
            }

            void playlist_ImageChanged(IPlaylist playlist, ImageEventArgs args)
            {
                if (ImageChanged != null)
                    ImageChanged(this, args);
            }

            void playlist_DescriptionChanged(IPlaylist playlist, DescriptionEventArgs args)
            {
                if (DescriptionChanged != null)
                    DescriptionChanged(this, args);
            }

            void playlist_TrackSeenChanged(IPlaylist playlist, TrackSeenEventArgs args)
            {
                if (TrackSeenChanged != null)
                    TrackSeenChanged(this, args);
            }

            void playlist_TrackCreatedChanged(IPlaylist playlist, TrackCreatedChangedEventArgs args)
            {
                if (TrackCreatedChanged != null)
                    TrackCreatedChanged(this, args);
            }

            void playlist_MetadataUpdated(IPlaylist playlist, EventArgs args)
            {
                if (MetadataUpdated != null)
                    MetadataUpdated(this, args);
            }

            void playlist_UpdateInProgress(IPlaylist playlist, PlaylistUpdateEventArgs args)
            {
                if (UpdateInProgress != null)
                    UpdateInProgress(this, args);
            }

            void playlist_StateChanged(IPlaylist playlist, EventArgs args)
            {
                if (StateChanged != null)
                    StateChanged(this, args);
            }

            void playlist_Renamed(IPlaylist playlist, EventArgs args)
            {
                if (Renamed != null)
                    Renamed(this, args);
            }

            void playlist_TracksMoved(IPlaylist playlist, TracksMovedEventArgs args)
            {
                if (TracksMoved != null)
                    TracksMoved(this, args);
            }

            void playlist_TracksRemoved(IPlaylist playlist, TracksEventArgs args)
            {
                if (TracksRemoved != null)
                    TracksRemoved(this, args);
            }

            void playlist_TracksAdded(IPlaylist playlist, TracksAddedEventArgs args)
            {
                if (TracksAdded != null)
                    TracksAdded(this, args);
            }

            protected override void OnDispose()
            {
                playlist.TracksAdded -= new PlaylistEventHandler<TracksAddedEventArgs>(playlist_TracksAdded);
                playlist.TracksRemoved -= new PlaylistEventHandler<TracksEventArgs>(playlist_TracksRemoved);
                playlist.TracksMoved -= new PlaylistEventHandler<TracksMovedEventArgs>(playlist_TracksMoved);
                playlist.Renamed -= new PlaylistEventHandler(playlist_Renamed);
                playlist.StateChanged -= new PlaylistEventHandler(playlist_StateChanged);
                playlist.UpdateInProgress -= new PlaylistEventHandler<PlaylistUpdateEventArgs>(playlist_UpdateInProgress);
                playlist.MetadataUpdated -= new PlaylistEventHandler(playlist_MetadataUpdated);
                playlist.TrackCreatedChanged -= new PlaylistEventHandler<TrackCreatedChangedEventArgs>(playlist_TrackCreatedChanged);
                playlist.TrackSeenChanged -= new PlaylistEventHandler<TrackSeenEventArgs>(playlist_TrackSeenChanged);
                playlist.DescriptionChanged -= new PlaylistEventHandler<DescriptionEventArgs>(playlist_DescriptionChanged);
                playlist.ImageChanged -= new PlaylistEventHandler<ImageEventArgs>(playlist_ImageChanged);
                if (this.GetType() == typeof(PlaylistWrapper))
                    Playlist.Delete(playlist.playlistPtr);
                playlist = null;
            }

            protected override int IntPtrHashCode()
            {
                return IsAlive() ? playlist.IntPtrHashCode() : 0;
            }

            public IPlaylistTrackList Tracks
            {
                get { IsAlive(true); return playlist.Tracks; }
            }

            public IPlaylistTrack this[int i]
            {
                get { IsAlive(true); return playlist[i]; }
            }

            public bool IsLoaded
            {
                get { IsAlive(true); return playlist.IsLoaded; }
            }

            public bool IsReady
            {
                get { IsAlive(true); return playlist.IsReady; }
            }

            public string Name
            {
                get { IsAlive(true); return playlist.Name; }
                set { IsAlive(true); playlist.Name = value; }
            }

            public ISession Session
            {
                get { IsAlive(true); return playlist.Session; }
            }

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


            public string ImageId
            {
                get { IsAlive(true); return playlist.ImageId; }
            }


            public bool IsColaberativ
            {
                get { IsAlive(true); return playlist.IsColaberativ; }
                set { IsAlive(true); playlist.IsColaberativ = value; }
            }

            public void AutoLinkTracks(bool autoLink)
            {
                IsAlive(true);
                playlist.AutoLinkTracks(autoLink);
            }

            public string Description
            {
                get { IsAlive(true); return playlist.Description; }
            }

            public bool PendingChanges
            {
                get { IsAlive(true); return playlist.PendingChanges; }
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
        private static Dictionary<IntPtr, Playlist> playlists = new Dictionary<IntPtr, Playlist>();
        private static readonly object playlistsLock = new object();

        internal static IPlaylist Get(Session session, IntPtr playlistPtr)
        {
            Playlist playlist;
            lock (playlistsLock)
            {
                if (!playlists.ContainsKey(playlistPtr))
                {
                    playlists.Add(playlistPtr, new Playlist(session, playlistPtr));
                }
                playlist = playlists[playlistPtr];
                playlist.AddRef();
            }
            return new PlaylistWrapper(playlist);
        }

        internal static void Delete(IntPtr playlistPtr)
        {
            lock (playlistsLock)
            {
                Playlist playlist = playlists[playlistPtr];
                int count = playlist.RemRef();
                if (count == 0)
                    playlists.Remove(playlistPtr);
            }
        }
        #endregion

        #region Spotify Callbacks
        private tracks_added_delegate tracks_added;
        private tracks_removed_delegate tracks_removed;
        private tracks_moved_delegate tracks_moved;
        private playlist_renamed_delegate playlist_renamed;
        private playlist_state_changed_delegate playlist_state_changed;
        private playlist_update_in_progress_delegate playlist_update_in_progress;
        private playlist_metadata_updated_delegate playlist_metadata_updated;
        private track_created_changed_delegate track_created_changed;
        private track_seen_changed_delegate track_seen_changed;
        private description_changed_delegate description_changed;
        private image_changed_delegate image_changed;
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

        private void UpdateInProgressCallback(IntPtr playlistPtr, bool complete, IntPtr userdataPtr)
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
                ITrack track = Track.Get(session, SPPlaylist.track(playlistPtr, position));
                //IUser user = User.Get(session, userPtr);
                DateTime dtWhen = new DateTime(TimeSpan.FromSeconds(when).Ticks, DateTimeKind.Utc);
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<TrackCreatedChangedEventArgs>(p => p.OnTrackCreatedChanged, this), new TrackCreatedChangedEventArgs(track, dtWhen)));
            }
        }

        private void TrackSeenChangedCallback(IntPtr playlistPtr, int position, bool seen, IntPtr userdataPtr)
        {
            if (playlistPtr == this.playlistPtr)
            {
                ITrack track = Track.Get(session, SPPlaylist.track(playlistPtr, position));
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<TrackSeenEventArgs>(p => p.OnTrackSeenChanged, this), new TrackSeenEventArgs(track, seen)));
            }
        }

        private void DescriptionChangedCallback(IntPtr playlistPtr, string description, IntPtr userdataPtr)
        {
            if (playlistPtr == this.playlistPtr)
            {
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
            PlaylistTrack.Update(this, aditions: args.TrackIndices);
            if (TracksAdded != null)
                TracksAdded(this, args);
        }
        protected virtual void OnTracksRemoved(TracksEventArgs args)
        {
            PlaylistTrack.Update(this, removals: args.TrackIndices);
            if (TracksRemoved != null)
                TracksRemoved(this, args);
        }
        protected virtual void OnTracksMoved(TracksMovedEventArgs args)
        {
            PlaylistTrack.Update(this, moves: args.TrackIndices, movedTo: args.NewPosition);
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
        private PlaylistCallbacks callbacks;
        private IntPtr callbacksPtr = IntPtr.Zero;
        private bool registerCallbacks;

        private IPlaylistTrackList tracks;
        #endregion

        #region Constructor
        protected Playlist(Session session, IntPtr playlistPtr, bool registerCallbacks = true)
        {
            if (playlistPtr == IntPtr.Zero)
                throw new ArgumentException("playlistPtr can't be zero.");

            if (session == null)
                throw new ArgumentNullException("Session can't be null.");

            this.session = session;
            this.playlistPtr = playlistPtr;
            this.registerCallbacks = registerCallbacks;

            if (registerCallbacks)
            {
                tracks_added = new tracks_added_delegate(TracksAddedCallback);
                tracks_removed = new tracks_removed_delegate(TracksRemovedCallback);
                tracks_moved = new tracks_moved_delegate(TracksMovedCallback);
                playlist_renamed = new playlist_renamed_delegate(RenamedCallback);
                playlist_state_changed = new playlist_state_changed_delegate(StateChangedCallback);
                playlist_update_in_progress = new playlist_update_in_progress_delegate(UpdateInProgressCallback);
                playlist_metadata_updated = new playlist_metadata_updated_delegate(MetadataUpdatedCallback);
                track_created_changed = new track_created_changed_delegate(TrackCreatedChangedCallback);
                track_seen_changed = new track_seen_changed_delegate(TrackSeenChangedCallback);
                description_changed = new description_changed_delegate(DescriptionChangedCallback);
                image_changed = new image_changed_delegate(ImageChangedCallback);
                callbacks = new PlaylistCallbacks
                {
                    tracks_added = Marshal.GetFunctionPointerForDelegate(tracks_added),
                    tracks_removed = Marshal.GetFunctionPointerForDelegate(tracks_removed),
                    tracks_moved = Marshal.GetFunctionPointerForDelegate(tracks_moved),
                    playlist_renamed = Marshal.GetFunctionPointerForDelegate(playlist_renamed),
                    playlist_state_changed = Marshal.GetFunctionPointerForDelegate(playlist_state_changed),
                    playlist_update_in_progress = Marshal.GetFunctionPointerForDelegate(playlist_update_in_progress),
                    playlist_metadata_updated = Marshal.GetFunctionPointerForDelegate(playlist_metadata_updated),
                    track_created_changed = Marshal.GetFunctionPointerForDelegate(track_created_changed),
                    track_seen_changed = Marshal.GetFunctionPointerForDelegate(track_seen_changed),
                    description_changed = Marshal.GetFunctionPointerForDelegate(description_changed),
                    image_changed = Marshal.GetFunctionPointerForDelegate(image_changed)
                };
                callbacksPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(PlaylistCallbacks)));
                Marshal.StructureToPtr(callbacks, callbacksPtr, false);
            }

            lock (libspotify.Mutex)
            {
                SPPlaylist.add_ref(playlistPtr);
                if (registerCallbacks)
                    SPPlaylist.add_callbacks(playlistPtr, callbacksPtr, IntPtr.Zero);
            }

            session.DisposeAll += new SessionEventHandler(session_DisposeAll);

            tracks = new PlaylistTrackList(
                () =>
                {
                    IsAlive(true);
                    lock (libspotify.Mutex)
                        return SPPlaylist.num_tracks(playlistPtr);
                },
                (index) =>
                {
                    IsAlive(true);
                    lock (libspotify.Mutex)
                        return PlaylistTrack.Get(session, this, SPPlaylist.track(playlistPtr, index), index);
                },
                (track, index) =>
                {
                    IsAlive(true);
                    lock (libspotify.Mutex)
                        SPPlaylist.add_tracks(playlistPtr, new[] { Track.GetPointer(track) }, index, session.sessionPtr);


                },
                (index) =>
                {
                    IsAlive(true);
                    IntPtr trackIndexArray = IntPtr.Zero;
                    try
                    {
                        trackIndexArray = Marshal.AllocHGlobal(sizeof(int));
                        Marshal.Copy(new[] { index }, 0, trackIndexArray, 1);

                        lock (libspotify.Mutex)
                            SPPlaylist.remove_tracks(playlistPtr, new[] { index });
                    }
                    finally
                    {
                        if (trackIndexArray != IntPtr.Zero)
                            Marshal.FreeHGlobal(trackIndexArray);
                    }
                },
                () => false
            );
            PlaylistTrack.RegisterPlaylist(this);
        }

        void session_DisposeAll(ISession sender, SessionEventArgs e)
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

        #region Internal Track Methods
        internal DateTime GetTrackCreationTime(PlaylistTrack track)
        {
            lock (libspotify.Mutex)
                return new DateTime(TimeSpan.FromSeconds(SPPlaylist.track_create_time(playlistPtr, track.position)).Ticks);
        }

        internal bool GetTrackSeen(PlaylistTrack track)
        {
            lock (libspotify.Mutex)
                return SPPlaylist.track_seen(playlistPtr, track.position);
        }
        #endregion

        #region Public Functions
        public void AutoLinkTracks(bool autoLink)
        {
            IsAlive(true);
            lock (libspotify.Mutex)
                SPPlaylist.set_autolink_tracks(playlistPtr, autoLink);
        }
        #endregion

        #region Properties
        public bool IsLoaded
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return SPPlaylist.is_loaded(playlistPtr);
            }
        }

        public virtual string Name
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return SPPlaylist.name(playlistPtr);
            }
            set
            {
                if (value.Length > 255)
                    throw new ArgumentException("value can't be longer than 255 chars.");
                IsAlive(true);
                lock (libspotify.Mutex)
                    SPPlaylist.rename(playlistPtr, value);
            }
        }

        public IPlaylistTrackList Tracks
        {
            get
            {
                IsAlive(true);
                return tracks;
            }
        }

        public IPlaylistTrack this[int i]
        {
            get
            {
                IsAlive(true);
                return tracks[i];
            }
        }

        public string ImageId
        {
            get
            {
                IsAlive(true);
                IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)) * 20);
                bool ans = false;
                lock (libspotify.Mutex)
                    ans = SPPlaylist.get_image(playlistPtr, ptr);
                try
                {
                    if (ans)
                    {
                        return libspotify.ImageIdToString(ptr);
                    }
                    else
                    {
                        return null;
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }

        public bool IsColaberativ
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return SPPlaylist.is_collaborative(playlistPtr);
            }
            set
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    SPPlaylist.set_collaborative(playlistPtr, value);
            }
        }

        public string Description
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return SPPlaylist.get_description(playlistPtr);
            }
        }

        public bool PendingChanges
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return SPPlaylist.has_pending_changes(playlistPtr);
            }
        }

        public ISession Session
        {
            get { IsAlive(true); return session; }
        }
        #endregion

        #region Cleanup
        protected override void OnDispose()
        {
            session.DisposeAll -= new SessionEventHandler(session_DisposeAll);
            PlaylistTrack.UnregisterPlaylist(this);
            if (!session.ProcExit)
                lock (libspotify.Mutex)
                {
                    if (registerCallbacks)
                    {
                        SPPlaylist.remove_callbacks(playlistPtr, callbacksPtr, IntPtr.Zero);
                    }
                    SPPlaylist.release(playlistPtr);
                }

            playlistPtr = IntPtr.Zero;
        }
        #endregion

        #region Await
        public bool IsReady
        {
            get { return !string.IsNullOrEmpty(Name) || !string.IsNullOrEmpty(ImageId) || !string.IsNullOrEmpty(Description) || playlistPtr == Playlist.GetPointer(session.Starred); }
        }
        #endregion

        protected override int IntPtrHashCode()
        {
            return playlistPtr.GetHashCode();
        }
    }
}

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
        #region Wrapper
        private class PlaylistWrapper: DisposeableSpotifyObject, IPlaylist
        {
            internal Playlist playlist;
            public PlaylistWrapper(Playlist playlist)
            {
                this.playlist = playlist;
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
        private tracks_added_cb                     tracks_added;
        private tracks_removed_cb                   tracks_removed;
        private tracks_moved_cd                     tracks_moved;
        private playlist_renamed_cb                 playlist_renamed;
        private playlist_state_changed_cb           playlist_state_changed;
        private playlist_update_in_progress_cb      playlist_update_in_progress;
        private playlist_metadata_updated_cb        playlist_metadata_updated;
        private track_created_changed_cb            track_created_changed;
        private track_seen_changed_cb               track_seen_changed;
        private description_changed_cb              description_changed;
        private image_changed_cb                    image_changed;
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
        #endregion

        #region Events
        public event PlaylistEventHandler<TracksAddedEventArgs> TracksAdded;
        public event PlaylistEventHandler<TracksEventArgs> TracksRemoved;
        public event PlaylistEventHandler<TracksMovedEventArgs> TracksMoved;
        public event PlaylistEventHandler Renamed;
        public event PlaylistEventHandler StateChanged;
        public event PlaylistEventHandler<PlaylistUpdateEventArgs> UpdateInProgress;
        public event PlaylistEventHandler MetadataUpdated;
        public event PlaylistEventHandler<TrackEventArgs> TrackCreatedChanged;
        public event PlaylistEventHandler<TrackEventArgs> TrackSeenChanged;
        public event PlaylistEventHandler<DescriptionEventArgs> DescriptionChanged;
        public event PlaylistEventHandler<ImageEventArgs> ImageChanged;
        #endregion

        #region Declarations
        internal IntPtr playlistPtr = IntPtr.Zero;
        private Session session;
        private sp_playlist_callbacks callbacks;
        private IntPtr callbacksPtr = IntPtr.Zero;
        #endregion

        #region Constructor
        private Playlist(Session session, IntPtr playlistPtr)
        {
            if (playlistPtr == IntPtr.Zero)
                throw new ArgumentException("playlistPtr can't be zero.");

            if (session == null)
                throw new ArgumentNullException("Session can't be null.");
            this.session = session;
            this.playlistPtr = playlistPtr;
            lock (libspotify.Mutex)
            {
                libspotify.sp_playlist_add_ref(playlistPtr);
                // TODO: Implement listeners
            }

            session.DisposeAll += new SessionEventHandler(session_DisposeAll);
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
        //private void ImageLoadedCallback(IntPtr imagePtr, IntPtr userdataPtr)
        //{
        //    if (imagePtr == this.imagePtr)
        //        session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<EventArgs>(i => i.OnLoaded, this), new EventArgs()));
        //}
        #endregion
    }
}

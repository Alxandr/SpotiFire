using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Linq.Expressions;

namespace SpotiFire.SpotifyLib
{
    public delegate void PlaylistContainerHandler(PlaylistContainer pc, EventArgs args);
    public delegate void PlaylistContainerHandler<TEventArgs>(PlaylistContainer pc, TEventArgs args) where TEventArgs : EventArgs;
    public class PlaylistContainer : DisposeableSpotifyObject
    {
        #region Structs
        private struct sp_playlistcontainer_callbacks
        {
            internal IntPtr playlist_added;
            internal IntPtr playlist_removed;
            internal IntPtr playlist_moved;
            internal IntPtr container_loaded;
        }
        #endregion

        #region Delegates
        private delegate void playlist_added_cb(IntPtr pcPtr, IntPtr playlistPtr, int position, IntPtr userdataPtr);
        private delegate void playlist_removed_cb(IntPtr pcPtr, IntPtr playlistPtr, int position, IntPtr userdataPtr);
        private delegate void playlist_moved_cb(IntPtr pcPtr, IntPtr playlistPtr, int position, int newPosition, IntPtr userdataPtr);
        private delegate void container_loaded_cb(IntPtr pcPtr, IntPtr userdataPtr);
        #endregion

        #region Declarations
        IntPtr pcPtr = IntPtr.Zero;
        private Session session;

        private sp_playlistcontainer_callbacks callbacks;
        private IntPtr callbacksPtr;
        private playlist_added_cb playlist_added;
        private playlist_removed_cb playlist_removed;
        private playlist_moved_cb playlist_moved;
        private container_loaded_cb container_loaded;
        #endregion

        #region Constructor
        internal PlaylistContainer(Session session, IntPtr pcPtr)
        {
            if (pcPtr == IntPtr.Zero)
                throw new ArgumentException("pcPtr can't be zero");

            if (session == null)
                throw new ArgumentNullException("Session can't be null");

            this.session = session;
            this.pcPtr = pcPtr;

            this.playlist_added = new playlist_added_cb(PlaylistAddedCallback);
            this.playlist_removed = new playlist_removed_cb(PlaylistRemovedCallback);
            this.playlist_moved = new playlist_moved_cb(PlaylistMovedCallback);
            this.container_loaded = new container_loaded_cb(ContainerLoadedCallback);
            this.callbacks = new sp_playlistcontainer_callbacks
            {
                playlist_added = Marshal.GetFunctionPointerForDelegate(playlist_added),
                playlist_removed = Marshal.GetFunctionPointerForDelegate(playlist_removed),
                playlist_moved = Marshal.GetFunctionPointerForDelegate(playlist_moved),
                container_loaded = Marshal.GetFunctionPointerForDelegate(container_loaded)
            };

            int size = Marshal.SizeOf(this.callbacks);
            this.callbacksPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(this.callbacks, this.callbacksPtr, true);

            lock (libspotify.Mutex)
            {
                libspotify.sp_playlistcontainer_add_callbacks(pcPtr, callbacksPtr, IntPtr.Zero);
            }
        }
        #endregion

        #region Private Methods
        private static Delegate CreateDelegate<T>(Expression<Func<PlaylistContainer, Action<T>>> expr, PlaylistContainer s) where T : EventArgs
        {
            return expr.Compile().Invoke(s);
        }
        private void PlaylistAddedCallback(IntPtr pcPtr, IntPtr playlistPtr, int position, IntPtr userdataPtr)
        {
            if (pcPtr == this.pcPtr)
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<PlaylistEventArgs>(pc => pc.OnPlaylistAdded, this), new PlaylistEventArgs(playlistPtr, position)));
        }
        private void PlaylistRemovedCallback(IntPtr pcPtr, IntPtr playlistPtr, int position, IntPtr userdataPtr)
        {
            if (pcPtr == this.pcPtr)
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<PlaylistEventArgs>(pc => pc.OnPlaylistRemoved, this), new PlaylistEventArgs(playlistPtr, position)));
        }
        private void PlaylistMovedCallback(IntPtr pcPtr, IntPtr playlistPtr, int position, int newPosition, IntPtr userdataPtr)
        {
            if (pcPtr == this.pcPtr)
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<PlaylistMovedEventArgs>(pc => pc.OnPlaylistMoved, this), new PlaylistEventArgs(playlistPtr, position)));
        }
        private void ContainerLoadedCallback(IntPtr pcPtr, IntPtr userdataPtr)
        {
            if (pcPtr == this.pcPtr)
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<EventArgs>(pc => pc.OnLoaded, this), new EventArgs()));
        }
        #endregion

        #region Protected Methods
        protected virtual void OnPlaylistAdded(PlaylistEventArgs args)
        {
            if (PlaylistAdded != null)
                PlaylistAdded(this, args);
        }
        protected virtual void OnPlaylistRemoved(PlaylistEventArgs args)
        {
            if (PlaylistRemoved != null)
                PlaylistRemoved(this, args);
        }
        protected virtual void OnPlaylistMoved(PlaylistMovedEventArgs args)
        {
            if (PlaylistMoved != null)
                PlaylistMoved(this, args);
        }
        protected virtual void OnLoaded(EventArgs args)
        {
            if (Loaded != null)
                Loaded(this, args);
        }
        #endregion

        #region Events
        public event PlaylistContainerHandler<PlaylistEventArgs> PlaylistAdded;
        public event PlaylistContainerHandler<PlaylistEventArgs> PlaylistRemoved;
        public event PlaylistContainerHandler<PlaylistMovedEventArgs> PlaylistMoved;
        public event PlaylistContainerHandler Loaded;
        #endregion

        #region Cleanup
        protected override void OnDispose()
        {
            lock (libspotify.Mutex)
            {
                libspotify.sp_playlistcontainer_remove_callbacks(pcPtr, callbacksPtr, IntPtr.Zero);
            }
            Marshal.FreeHGlobal(callbacksPtr);
            callbacksPtr = IntPtr.Zero;
            pcPtr = IntPtr.Zero;
        }
        #endregion

        protected override int IntPtrHashCode()
        {
            return pcPtr.GetHashCode();
        }
    }
}

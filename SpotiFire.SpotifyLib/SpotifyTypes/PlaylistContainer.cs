using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using SPPlaylistcontainer = SpotiFire.Playlistcontainer;

namespace SpotiFire
{
    public delegate void PlaylistContainerHandler(IPlaylistContainer pc, EventArgs args);
    public delegate void PlaylistContainerHandler<TEventArgs>(IPlaylistContainer pc, TEventArgs args) where TEventArgs : EventArgs;
}

namespace SpotiFire.Types
{
    internal class PlaylistContainer : CountedDisposeableSpotifyObject, IPlaylistContainer, ISpotifyAwaitable
    {
        #region Wrapper
        private class PlaylistContainerWrapper : DisposeableSpotifyObject, IPlaylistContainer, ISpotifyAwaitable
        {
            internal PlaylistContainer pc;
            public PlaylistContainerWrapper(PlaylistContainer pc)
            {
                this.pc = pc;
                pc.Loaded += new PlaylistContainerHandler(pc_Loaded);
                pc.PlaylistAdded += new PlaylistContainerHandler<PlaylistEventArgs>(pc_PlaylistAdded);
                pc.PlaylistMoved += new PlaylistContainerHandler<PlaylistMovedEventArgs>(pc_PlaylistMoved);
                pc.PlaylistRemoved += new PlaylistContainerHandler<PlaylistEventArgs>(pc_PlaylistRemoved);
            }

            void pc_PlaylistRemoved(IPlaylistContainer pc, PlaylistEventArgs args)
            {
                if (PlaylistRemoved != null)
                    PlaylistRemoved(this, args);
            }

            void pc_PlaylistMoved(IPlaylistContainer pc, PlaylistMovedEventArgs args)
            {
                if (PlaylistMoved != null)
                    PlaylistMoved(this, null);
            }

            void pc_PlaylistAdded(IPlaylistContainer pc, PlaylistEventArgs args)
            {
                if (PlaylistAdded != null)
                    PlaylistAdded(this, args);
            }

            void pc_Loaded(IPlaylistContainer pc, EventArgs args)
            {
                if (Loaded != null)
                    Loaded(this, args);
            }

            protected override void OnDispose()
            {
                pc.Loaded -= new PlaylistContainerHandler(pc_Loaded);
                pc.PlaylistAdded -= new PlaylistContainerHandler<PlaylistEventArgs>(pc_PlaylistAdded);
                pc.PlaylistMoved -= new PlaylistContainerHandler<PlaylistMovedEventArgs>(pc_PlaylistMoved);
                pc.PlaylistRemoved -= new PlaylistContainerHandler<PlaylistEventArgs>(pc_PlaylistRemoved);
                PlaylistContainer.Delete(pc.pcPtr);
                pc = null;
            }

            protected override int IntPtrHashCode()
            {
                return IsAlive() ? pc.IntPtrHashCode() : 0;
            }

            public event PlaylistContainerHandler Loaded;

            public event PlaylistContainerHandler<PlaylistEventArgs> PlaylistAdded;

            public event PlaylistContainerHandler<PlaylistMovedEventArgs> PlaylistMoved;

            public event PlaylistContainerHandler<PlaylistEventArgs> PlaylistRemoved;

            public ISession Session
            {
                get { IsAlive(true); return pc.Session; }
            }

            public IPlaylistList Playlists
            {
                get { IsAlive(true); return pc.Playlists; }
            }

            public IContainerPlaylist this[int i]
            {
                get { IsAlive(true); return pc[i]; }
            }

            public bool IsLoaded
            {
                get { IsAlive(true); return pc.IsLoaded; }
            }

            bool ISpotifyAwaitable.IsComplete
            {
                get { IsAlive(true); return ((ISpotifyAwaitable)pc).IsComplete; }
            }

            bool ISpotifyAwaitable.AddContinuation(AwaitHelper.Continuation continuation, bool addBeforeOthers)
            {
                IsAlive(true); return ((ISpotifyAwaitable)pc).AddContinuation(continuation, addBeforeOthers);
            }
        }

        internal static IntPtr GetPointer(IPlaylistContainer pc)
        {
            if (pc.GetType() == typeof(PlaylistContainerWrapper))
                return ((PlaylistContainerWrapper)pc).pc.pcPtr;
            throw new ArgumentException("Invalid pc");
        }
        #endregion
        #region Counter
        private static Dictionary<IntPtr, PlaylistContainer> pcs = new Dictionary<IntPtr, PlaylistContainer>();
        private static readonly object pcsLock = new object();

        internal static IPlaylistContainer Get(Session session, IntPtr pcPtr)
        {
            PlaylistContainer pc;
            lock (pcsLock)
            {
                if (!pcs.ContainsKey(pcPtr))
                {
                    pcs.Add(pcPtr, new PlaylistContainer(session, pcPtr));
                }
                pc = pcs[pcPtr];
                pc.AddRef();
            }
            return new PlaylistContainerWrapper(pc);
        }

        internal static void Delete(IntPtr pcPtr)
        {
            lock (pcsLock)
            {
                PlaylistContainer pc = pcs[pcPtr];
                int count = pc.RemRef();
                if (count == 0)
                    pcs.Remove(pcPtr);
            }
        }
        #endregion

        #region Declarations
        IntPtr pcPtr = IntPtr.Zero;
        private Session session;

        private PlaylistContainerCallbacks callbacks;
        private IntPtr callbacksPtr;
        private playlist_added_delegate playlist_added;
        private playlist_removed_delegate playlist_removed;
        private playlist_moved_delegate playlist_moved;
        private container_loaded_delegate container_loaded;

        private PlaylistList playlists;
        private bool loaded;
        #endregion

        #region Constructor
        private PlaylistContainer(Session session, IntPtr pcPtr)
        {
            if (pcPtr == IntPtr.Zero)
                throw new ArgumentException("pcPtr can't be zero");

            if (session == null)
                throw new ArgumentNullException("Session can't be null");

            this.session = session;
            this.pcPtr = pcPtr;

            this.playlist_added = new playlist_added_delegate(PlaylistAddedCallback);
            this.playlist_removed = new playlist_removed_delegate(PlaylistRemovedCallback);
            this.playlist_moved = new playlist_moved_delegate(PlaylistMovedCallback);
            this.container_loaded = new container_loaded_delegate(ContainerLoadedCallback);
            this.callbacks = new PlaylistContainerCallbacks
            {
                playlist_added = Marshal.GetFunctionPointerForDelegate(playlist_added),
                playlist_removed = Marshal.GetFunctionPointerForDelegate(playlist_removed),
                playlist_moved = Marshal.GetFunctionPointerForDelegate(playlist_moved),
                container_loaded = Marshal.GetFunctionPointerForDelegate(container_loaded)
            };

            int size = Marshal.SizeOf(this.callbacks);
            this.callbacksPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(callbacks, callbacksPtr, true);

            lock (libspotify.Mutex)
            {
                SPPlaylistcontainer.add_callbacks(pcPtr, callbacksPtr, IntPtr.Zero);
            }

            session.DisposeAll += new SessionEventHandler(session_DisposeAll);

            playlists = new PlaylistList(
                () =>
                {
                    IsAlive(true);
                    lock (libspotify.Mutex)
                        return SPPlaylistcontainer.num_playlists(pcPtr);
                },
                (index) =>
                {
                    IsAlive(true);
                    lock (libspotify.Mutex)
                        return ContainerPlaylist.Get(session, this, SPPlaylistcontainer.playlist(pcPtr, index),
                            SPPlaylistcontainer.playlist_folder_id(pcPtr, index), (PlaylistType)SPPlaylistcontainer.playlist_type(pcPtr, index));
                },
                (playlist, index) =>
                {
                    IsAlive(true);
                    IntPtr playlistPtr;
                    int newIndex = 0;
                    lock (libspotify.Mutex)
                    {
                        playlistPtr = SPPlaylistcontainer.add_new_playlist(pcPtr, playlist.Name);
                        newIndex = SPPlaylistcontainer.num_playlists(pcPtr);
                    }
                    if (playlistPtr != IntPtr.Zero)
                        lock (libspotify.Mutex)
                            SPPlaylistcontainer.move_playlist(pcPtr, newIndex, index, false);
                },
                (index) =>
                {
                    IsAlive(true);
                    lock (libspotify.Mutex)
                        SPPlaylistcontainer.remove_playlist(pcPtr, index);
                },
                () => false,
                (name) =>
                {
                    IsAlive(true);
                    IntPtr playlistPtr;
                    int index;
                    lock (libspotify.Mutex)
                    {
                        playlistPtr = SPPlaylistcontainer.add_new_playlist(pcPtr, name);
                        index = playlistPtr == IntPtr.Zero ? -1 : SPPlaylistcontainer.num_playlists(pcPtr) - 1;
                    }
                    return index == -1 ? null : playlists[index];
                }
            );
        }

        void session_DisposeAll(ISession sender, SessionEventArgs e)
        {
            Dispose();
        }
        #endregion

        #region Properties
        public IPlaylistList Playlists
        {
            get { return playlists; }
        }

        public IContainerPlaylist this[int i]
        {
            get { return playlists[i]; }
        }

        public ISession Session
        {
            get
            {
                IsAlive(true);
                return session;
            }
        }

        public bool IsLoaded
        {
            get
            {
                IsAlive(true);
                return loaded;
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
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<PlaylistMovedEventArgs>(pc => pc.OnPlaylistMoved, this), new PlaylistMovedEventArgs(playlistPtr, position, newPosition)));
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
            lock (this)
                loaded = true;
            if (Loaded != null)
                Loaded(this, args);
        }
        #endregion

        #region Internal Playlist methods
        internal string GetFolderName(ContainerPlaylist playlist)
        {
            int index = playlists.IndexOf(playlist);
            return SPPlaylistcontainer.playlist_folder_name(pcPtr, index);
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
            session.DisposeAll -= new SessionEventHandler(session_DisposeAll);

            if (!session.ProcExit)
            {
                lock (libspotify.Mutex)
                {
                    SPPlaylistcontainer.remove_callbacks(pcPtr, callbacksPtr, IntPtr.Zero);
                }
            }
            pcPtr = IntPtr.Zero;
        }
        #endregion

        #region Await
        bool ISpotifyAwaitable.IsComplete
        {
            get
            {
                lock (this)
                    return IsLoaded;
            }
        }

        PlaylistContainerHandler _handler;
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
                        Loaded -= _handler;
                        _handler = null;
                    }
                };
                Loaded += _handler;
            }
        }

        bool ISpotifyAwaitable.AddContinuation(AwaitHelper.Continuation continuation, bool addBeforeOthers)
        {
            lock (this)
            {
                if (((ISpotifyAwaitable)this).IsComplete)
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

        protected override int IntPtrHashCode()
        {
            return pcPtr.GetHashCode();
        }


    }
}

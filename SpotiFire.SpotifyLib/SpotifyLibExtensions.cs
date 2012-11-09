using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SpotiFire
{

    public static class SessionExtensions
    {
        // Image methods
        public static Image GetImageFromId(this Session session, string id)
        {
            throw new NotImplementedException();
            //return Image.FromId(session, id);
        }

        public static System.Drawing.Image GetImage(this Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            if (!image.IsLoaded)
                throw new InvalidOperationException("Can't convert to Image before data is loaded.");

            if (image.Format != ImageFormat.Jpeg)
                throw new InvalidOperationException("Can only parse Jpeg image data.");

            throw new NotImplementedException();
            //return System.Drawing.Image.FromStream(new MemoryStream(image.Data));
        }

        public static void Save(this Image image, string location)
        {
            image.GetImage().Save(location);
        }

        public static AwaitHelper.AwaitableAwaiter<Search> GetAwaiter(this Search search)
        {
            return AwaitHelper.GetAwaiter<Search>(search);
        }

        public static Search Search(this Session session,
            string query,
            int trackOffset, int trackCount,
            int albumOffset, int albumCount,
            int artistOffset, int artistCount,
            int playlistOffset, int playlistCount,
            SearchType type)
        {
            return SpotiFire.Search.Create(session, query, trackOffset, trackCount, albumOffset, albumCount, artistOffset, artistCount, playlistOffset, playlistCount, type);
        }

        public static Search SearchTracks(this Session session, string query, int trackOffset, int trackCount)
        {
            return Search(session, query, trackOffset, trackCount, 0, 0, 0, 0, 0, 0, SearchType.Standard);
        }
        public static Search SearchAlbums(this Session session, string query, int albumOffset, int albumCount)
        {
            return Search(session, query, 0, 0, albumOffset, albumCount, 0, 0, 0, 0, SearchType.Standard);
        }
        public static Search SearchArtists(this Session session, string query, int artistOffset, int artistCount)
        {
            return Search(session, query, 0, 0, 0, 0, artistOffset, artistCount, 0, 0, SearchType.Standard);
        }
        public static Search SearchPlaylist(this Session session, string query, int playlistOffset, int playlistCount)
        {
            return Search(session, query, 0, 0, 0, 0, 0, 0, playlistOffset, playlistCount, SearchType.Standard);
        }

        // ArtistBrowse methods made Synchronously
        public static AwaitHelper.AwaitableAwaiter<ArtistBrowse> GetAwaiter(this ArtistBrowse artistBrowse)
        {
            return AwaitHelper.GetAwaiter<ArtistBrowse>(artistBrowse);
        }

        // AlbumBrowse methods made Synchronously
        public static AwaitHelper.AwaitableAwaiter<AlbumBrowse> GetAwaiter(this AlbumBrowse albumBrowse)
        {
            return AwaitHelper.GetAwaiter<AlbumBrowse>(albumBrowse);
        }

        // PlaylistContainer
        public static AwaitHelper.AwaitableAwaiter<PlaylistContainer> GetAwaiter(this PlaylistContainer pc)
        {
            return AwaitHelper.GetAwaiter<PlaylistContainer>(pc);
        }

        private readonly static List<Tuple<IAsyncLoaded, TaskCompletionSource<IAsyncLoaded>>> _waiting = new List<Tuple<IAsyncLoaded, TaskCompletionSource<IAsyncLoaded>>>();
        private readonly static Timer _timer = new Timer(_timer_Tick, new List<Tuple<IAsyncLoaded, TaskCompletionSource<IAsyncLoaded>>>(), Timeout.Infinite, 100);
        private volatile static bool _running = false;

        private static void _timer_Tick(object _state)
        {
            List<Tuple<IAsyncLoaded, TaskCompletionSource<IAsyncLoaded>>> waiting = (List<Tuple<IAsyncLoaded, TaskCompletionSource<IAsyncLoaded>>>)_state;
            lock (_waiting)
            {
                if (_waiting.Count > 0)
                {
                    waiting.AddRange(_waiting);
                    _waiting.Clear();
                }
            }

            if (waiting.Count > 0)
            {
                for (var i = 0; i < waiting.Count; i++)
                {
                    var w = waiting[i];
                    if (w.Item1.IsLoaded && w.Item1.IsReady)
                    {
                        w.Item2.SetResult(w.Item1);
                        waiting.RemoveAt(i);
                        i--;
                    }
                }
            }
            else
            {
                lock (_waiting)
                {
                    if (_waiting.Count == 0)
                    {
                        _timer.Change(Timeout.Infinite, 100);
                        _running = false;
                    }
                }
            }
        }

        // Load made a task
        private static Task<IAsyncLoaded> Load(this IAsyncLoaded loadable)
        {
            TaskCompletionSource<IAsyncLoaded> tcs = new TaskCompletionSource<IAsyncLoaded>();
            if (loadable.IsLoaded && loadable.IsReady)
            {
                tcs.SetResult(loadable);
            }
            else
            {
                lock (_waiting)
                {
                    _waiting.Add(new Tuple<IAsyncLoaded, TaskCompletionSource<IAsyncLoaded>>(loadable, tcs));
                    if (!_running)
                    {
                        _running = true;
                        _timer.Change(0, 100);
                    }
                }
            }
            return tcs.Task;
        }

        public static TaskAwaiter<Track> GetAwaiter(this Track track)
        {
            return ((IAsyncLoaded)track).Load().ContinueWith(task => (Track)task.Result).GetAwaiter();
        }

        public static TaskAwaiter<Artist> GetAwaiter(this Artist artist)
        {
            return ((IAsyncLoaded)artist).Load().ContinueWith(task => (Artist)task.Result).GetAwaiter();
        }

        public static TaskAwaiter<Album> GetAwaiter(this Album album)
        {
            return ((IAsyncLoaded)album).Load().ContinueWith(task => (Album)task.Result).GetAwaiter();
        }

        public static TaskAwaiter<Playlist> GetAwaiter(this Playlist playlist)
        {
            return ((IAsyncLoaded)playlist).Load().ContinueWith(task => (Playlist)task.Result).GetAwaiter();
        }

        // play through track
        public static Task Play(this Session session, Track track)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            SessionEventHandler handler = null;
            handler = (s, e) =>
            {
                session.EndOfTrack -= handler;
                tcs.SetResult(null);
            };

            session.PlayerUnload();
            session.EndOfTrack += handler;
            session.PlayerLoad(track);
            session.PlayerPlay();

            return tcs.Task;
        }
    }

}

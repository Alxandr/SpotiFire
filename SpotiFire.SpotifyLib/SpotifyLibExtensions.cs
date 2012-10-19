using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SpotiFire
{

    public static class ISessionExtensions
    {
        // IImage methods
        public static IImage GetImageFromId(this ISession session, string id)
        {
            return Types.Image.FromId(session, id);
        }

        public static System.Drawing.Image GetImage(this IImage image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            if (!image.IsLoaded)
                throw new InvalidOperationException("Can't convert to Image before data is loaded.");

            if (image.Format != ImageFormat.JPEG)
                throw new InvalidOperationException("Can only parse Jpeg image data.");

            return System.Drawing.Image.FromStream(new MemoryStream(image.Data));
        }

        public static void Save(this IImage image, string location)
        {
            image.GetImage().Save(location);
        }

        // IImage Synchronously
        public static void WaitForLoaded(this IImage image)
        {
            var reset = new ManualResetEvent(image.IsLoaded);
            ImageEventHandler handler = (i, e) => reset.Set();
            image.Loaded += handler;
            reset.WaitOne();
            image.Loaded -= handler;
        }

        public static ISpotifyAwaiter<ISearch> GetAwaiter(this ISearch search)
        {
            return AwaitHelper.GetAwaiter<ISearch>(search);
        }

        public static ISearch SearchTracks(this ISession session, string query, int trackOffset, int trackCount)
        {
            return session.Search(query, trackOffset, trackCount, 0, 0, 0, 0, 0, 0, SearchType.Standard);
        }
        public static ISearch SearchAlbums(this ISession session, string query, int albumOffset, int albumCount)
        {
            return session.Search(query, 0, 0, albumOffset, albumCount, 0, 0, 0, 0, SearchType.Standard);
        }
        public static ISearch SearchArtists(this ISession session, string query, int artistOffset, int artistCount)
        {
            return session.Search(query, 0, 0, 0, 0, artistOffset, artistCount, 0, 0, SearchType.Standard);
        }
        public static ISearch SearchPlaylist(this ISession session, string query, int playlistOffset, int playlistCount)
        {
            return session.Search(query, 0, 0, 0, 0, 0, 0, playlistOffset, playlistCount, SearchType.Standard);
        }

        // ArtistBrowse methods made Synchronously
        public static ISpotifyAwaiter<IArtistBrowse> GetAwaiter(this IArtistBrowse artistBrowse)
        {
            return AwaitHelper.GetAwaiter<IArtistBrowse>(artistBrowse);
        }

        // AlbumBrowse methods made Synchronously
        public static ISpotifyAwaiter<IAlbumBrowse> GetAwaiter(this IAlbumBrowse albumBrowse)
        {
            return AwaitHelper.GetAwaiter<IAlbumBrowse>(albumBrowse);
        }

        // PlaylistContainer
        public static ISpotifyAwaiter<IPlaylistContainer> GetAwaiter(this IPlaylistContainer pc)
        {
            return AwaitHelper.GetAwaiter<IPlaylistContainer>(pc);
        }

        // Load made a task
        private readonly static List<Tuple<IAsyncLoaded, TaskCompletionSource<IAsyncLoaded>>> waiting = new List<Tuple<IAsyncLoaded, TaskCompletionSource<IAsyncLoaded>>>();
        private static bool running = false;
        private static Task<IAsyncLoaded> Load(this IAsyncLoaded loadable)
        {
            Action start = () =>
            {
                running = true;
                ThreadPool.QueueUserWorkItem(delegate
                {
                    while (true)
                    {
                        Tuple<IAsyncLoaded, TaskCompletionSource<IAsyncLoaded>>[] l;
                        Thread.Sleep(100);
                        lock (waiting)
                            l = waiting.ToArray();

                        HashSet<Tuple<IAsyncLoaded, TaskCompletionSource<IAsyncLoaded>>> r = new HashSet<Tuple<IAsyncLoaded, TaskCompletionSource<IAsyncLoaded>>>();

                        foreach (var i in l)
                            if (i.Item1.IsLoaded && i.Item1.IsReady)
                            {
                                i.Item2.SetResult(i.Item1);
                                r.Add(i);
                            }

                        lock (waiting)
                        {
                            waiting.RemoveAll(e => r.Contains(e));
                            if (waiting.Count == 0)
                            {
                                running = false;
                                break;
                            }
                        }
                    }
                });
            };

            TaskCompletionSource<IAsyncLoaded> tcs = new TaskCompletionSource<IAsyncLoaded>();
            if (loadable.IsLoaded && loadable.IsReady)
                tcs.SetResult(loadable);
            else if (running)
                lock (waiting)
                    if (running)
                        waiting.Add(new Tuple<IAsyncLoaded, TaskCompletionSource<IAsyncLoaded>>(loadable, tcs));
                    else
                    {
                        waiting.Add(new Tuple<IAsyncLoaded, TaskCompletionSource<IAsyncLoaded>>(loadable, tcs));
                        start();
                    }
            else
                lock (waiting)
                {
                    waiting.Add(new Tuple<IAsyncLoaded, TaskCompletionSource<IAsyncLoaded>>(loadable, tcs));
                    start();
                }

            return tcs.Task;
        }

        public static TaskAwaiter<ITrack> GetAwaiter(this ITrack track)
        {
            return ((IAsyncLoaded)track).Load().ContinueWith(task => (ITrack)task.Result).GetAwaiter();
        }

        public static TaskAwaiter<IArtist> GetAwaiter(this IArtist artist)
        {
            return ((IAsyncLoaded)artist).Load().ContinueWith(task => (IArtist)task.Result).GetAwaiter();
        }

        public static TaskAwaiter<IAlbum> GetAwaiter(this IAlbum album)
        {
            return ((IAsyncLoaded)album).Load().ContinueWith(task => (IAlbum)task.Result).GetAwaiter();
        }

        public static TaskAwaiter<IPlaylist> GetAwaiter(this IPlaylist playlist)
        {
            return ((IAsyncLoaded)playlist).Load().ContinueWith(task => (IPlaylist)task.Result).GetAwaiter();
        }

        // play through track
        public static Task Play(this ISession session, ITrack track)
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

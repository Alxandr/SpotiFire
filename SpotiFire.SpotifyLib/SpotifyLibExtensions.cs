using System;
using System.IO;
using System.Threading;

namespace SpotiFire.SpotifyLib
{

    public static class ISessionExtensions
    {
        // IImage methods
        public static IImage GetImageFromId(this ISession session, string id)
        {
            return Image.FromId(session, id);
        }

        public static System.Drawing.Image GetImage(this IImage image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            if (!image.IsLoaded)
                throw new InvalidOperationException("Can't convert to Image before data is loaded.");

            if (image.Format != sp_imageformat.SP_IMAGE_FORMAT_JPEG)
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


        // Search methods made Synchronously
        public static void WaitForCompletion(this ISearch search)
        {
            var reset = new ManualResetEvent(search.IsComplete);
            SearchEventHandler handler = (s, e) => reset.Set();
            search.Complete += handler;
            reset.WaitOne();
            search.Complete -= handler;
        }
        public static ISearch SearchTracks(this ISession session, string query, int trackOffset, int trackCount)
        {
            return session.Search(query, trackOffset, trackCount, 0, 0, 0, 0);
        }
        public static ISearch SearchAlbums(this ISession session, string query, int albumOffset, int albumCount)
        {
            return session.Search(query, 0, 0, albumOffset, albumCount, 0, 0);
        }
        public static ISearch SearchArtists(this ISession session, string query, int artistOffset, int artistCount)
        {
            return session.Search(query, 0, 0, 0, 0, artistOffset, artistCount);
        }

        // ArtistBrowse methods made Synchronously
        public static void WaitForCompletion(this IArtistBrowse artistBrowse)
        {
            var reset = new ManualResetEvent(artistBrowse.IsComplete);
            ArtistBrowseEventHandler handler = (a, e) => reset.Set();
            artistBrowse.Complete += handler;
            reset.WaitOne();
            artistBrowse.Complete -= handler;
        }

        // AlbumBrowse methods made Synchronously
        public static void WaitForCompletion(this IAlbumBrowse albumBrowse)
        {
            var reset = new ManualResetEvent(albumBrowse.IsComplete);
            AlbumBrowseEventHandler handler = (a, e) => reset.Set();
            albumBrowse.Complete += handler;
            reset.WaitOne();
            albumBrowse.Complete -= handler;
        }
    }

}

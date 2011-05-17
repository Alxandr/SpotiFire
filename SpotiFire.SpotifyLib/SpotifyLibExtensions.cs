using System;
using System.Threading;

namespace SpotiFire.SpotifyLib {

    public static class ISessionExtensions {

        // Search methods made Synchronously
        public static ISearch SearchSynchronously(this ISession session, string query, int trackOffset, int trackCount, int albumOffset, int albumCount, int artistOffset, int artistCount, object state = null) {
            var reset = new ManualResetEvent(false);
            var search = session.Search(query, trackOffset, trackCount, albumOffset, albumCount, artistOffset, artistCount, state);
            if (!search.IsComplete) {
                search.Complete += (sender, evt) => {
                    reset.Set();
                };
                reset.WaitOne();
            }
            return search;
        }
        public static ISearch SearchTracksSynchronously(this ISession session, string query, int trackOffset, int trackCount, object state = null) {
            return session.SearchSynchronously(query, trackOffset, trackCount, 0, 0, 0, 0, state);
        }
        public static ISearch SearchAlbumsSynchronously(this ISession session, string query, int albumOffset, int albumCount, object state = null) {
            return session.SearchSynchronously(query, 0, 0, albumOffset, albumCount, 0, 0, state);
        }
        public static ISearch SearchArtistsSynchronously(this ISession session, string query, int artistOffset, int artistCount, object state = null) {
            return session.SearchSynchronously(query, 0, 0, 0, 0, artistOffset, artistCount, state);
        }

        // Browse methods made Synchronously
        public static IArtistBrowse BrowseSynchronously(this IArtist artist, object state = null) {
            var reset = new ManualResetEvent(false);
            var artistBrowse = artist.Browse(state);
            if (!artistBrowse.IsComplete) {
                artistBrowse.Complete += (sender, evt) => {
                    reset.Set();
                };
                reset.WaitOne();
            }
            return artistBrowse;
        }

    }

}

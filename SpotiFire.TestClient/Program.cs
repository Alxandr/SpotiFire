using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpotiFire.TestClient
{
    class Program
    {
        static byte[] key = new byte[] {
    0x01, 0xF7, 0x8F, 0x3C, 0x44, 0x98, 0xF7, 0xD3, 0xD6, 0xF5, 0xD2, 0x40, 0x23, 0x02, 0x98, 0x1B,
    0x30, 0x6B, 0xE4, 0xCC, 0x82, 0xB3, 0x97, 0x2A, 0xE1, 0x42, 0x1F, 0x90, 0x3B, 0xE4, 0xCD, 0x25,
    0xD5, 0x90, 0xBC, 0x2C, 0xCD, 0x48, 0x33, 0xB8, 0x13, 0xC0, 0xD7, 0x1C, 0x94, 0xBD, 0x16, 0x1E,
    0xCD, 0xAD, 0x3D, 0x1C, 0xCA, 0xAF, 0xF0, 0x6D, 0x06, 0x64, 0x87, 0xA4, 0xBC, 0x32, 0x13, 0xFE,
    0x2C, 0xCB, 0x9C, 0x14, 0xD7, 0xB4, 0x98, 0x27, 0x31, 0x12, 0x18, 0x52, 0xD5, 0x1C, 0xA6, 0x4C,
    0x86, 0x08, 0xA5, 0xE8, 0x69, 0x20, 0xE3, 0x3F, 0x4A, 0xF4, 0xF1, 0xB6, 0x00, 0xA6, 0x35, 0x5A,
    0x5B, 0x20, 0x24, 0x1C, 0x14, 0x43, 0x52, 0x49, 0xDE, 0x58, 0xC8, 0x8E, 0x00, 0x2B, 0x4C, 0x62,
    0xB7, 0xB4, 0x58, 0x79, 0xF2, 0x75, 0x8A, 0x10, 0x43, 0x93, 0x80, 0xA9, 0xB9, 0x7A, 0x66, 0xD6,
    0x91, 0x36, 0xA6, 0x70, 0x6C, 0x9F, 0xF4, 0x54, 0x2A, 0x1D, 0x5D, 0x35, 0x99, 0xB8, 0xDB, 0xD4,
    0x3A, 0x41, 0x05, 0x5F, 0x36, 0x68, 0xFA, 0x08, 0xED, 0x7F, 0xA5, 0x29, 0x49, 0x78, 0x5C, 0xEA,
    0x8E, 0x32, 0x2D, 0xEF, 0x71, 0xE2, 0xD2, 0x85, 0x56, 0xAD, 0xF9, 0xAA, 0x58, 0x33, 0xF2, 0xD2,
    0xC6, 0x8C, 0x04, 0x0C, 0x1E, 0xE1, 0x01, 0xCC, 0x97, 0x11, 0x2C, 0xD9, 0x1F, 0x89, 0x8C, 0xC9,
    0xCE, 0xA7, 0xF1, 0xCA, 0x15, 0xCA, 0x62, 0x15, 0x34, 0x2D, 0x33, 0x94, 0x6B, 0x23, 0x0E, 0xCA,
    0xC7, 0x24, 0xD3, 0xF7, 0xE6, 0xB4, 0x16, 0xC1, 0xDC, 0x26, 0x45, 0x45, 0xA5, 0x7A, 0x62, 0xF9,
    0xC5, 0x8F, 0xF0, 0xD3, 0x6F, 0x0C, 0x7C, 0xA8, 0x10, 0x49, 0xAE, 0x5F, 0x24, 0x88, 0xA9, 0xE3,
    0x69, 0x66, 0x25, 0x2E, 0x59, 0xA7, 0x62, 0xA7, 0x80, 0xF3, 0x61, 0x7C, 0x75, 0xF7, 0x40, 0x33,
    0x95, 0xAF, 0x66, 0x15, 0x5A, 0x27, 0xF2, 0x6B, 0x2C, 0x41, 0x58, 0x6A, 0x88, 0x15, 0x13, 0xE6,
    0xDB, 0x45, 0x50, 0x94, 0x79, 0x48, 0x43, 0xB1, 0x42, 0x8D, 0x5F, 0xB6, 0x49, 0xD3, 0x9F, 0x00,
    0xE9, 0xEE, 0x06, 0xBE, 0x7B, 0x42, 0x47, 0x0C, 0x30, 0xA3, 0x6F, 0xA0, 0x9B, 0xC3, 0x03, 0xB8,
    0x82, 0x42, 0x24, 0x8A, 0x78, 0x0D, 0x1D, 0xFA, 0x26, 0x65, 0x62, 0x0D, 0x4D, 0xCC, 0x3B, 0xF9,
    0x1A
        };

        static string cache = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "spotifire", "cache");
        static string settings = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "spotifire", "settings");
        static string userAgent = "SpotiFire";
        static IPlayer player = new NAudioPlayer();

        static void Main(string[] args)
        {
            Run().Wait();
        }

        static async Task PlayQueueTest()
        {
            Console.WriteLine("Enter username and password (a line for each)");
            Session session = await Spotify.CreateSession(key, cache, settings, userAgent);
            session.MusicDelivered += session_MusicDeliver;
            Error err;
            do
            {
                err = await session.Login(Console.ReadLine(), Console.ReadLine(), false);
                if (err != Error.OK)
                {
                    Console.WriteLine("An error occurred while logging in.\n{0}", err.Message());
                }
            } while (err != Error.OK);

            session.PreferredBitrate = BitRate.Bitrate320k;

            await session.PlaylistContainer;
            while (session.PlaylistContainer.Playlists.Count < 1)
            {
                Console.WriteLine("Found {0} playlists, retrying in 2 sec.", session.PlaylistContainer.Playlists.Count);
                await Task.Delay(TimeSpan.FromSeconds(2));
            }

            AutoResetEvent are = new AutoResetEvent(false);

            PlayQueue queue = new PlayQueue();
            session.EndOfTrack += (s, e) =>
            {
                if (!queue.IsEmpty)
                {
                    var track = queue.Dequeue();
                    session.PlayerUnload();
                    session.PlayerLoad(track);
                    session.PlayerPlay();
                }
                else
                {
                    are.Set();
                }
            };

            var playlist = await session.PlaylistContainer.Playlists[0];
            queue.Seed = playlist.Tracks;
            if (!queue.IsEmpty)
            {
                var track = await queue.Dequeue();
                session.PlayerUnload();
                session.PlayerLoad(track);
                session.PlayerPlay();
                are.WaitOne();
            }
        }

        static async Task Run()
        {
            Console.WriteLine("Enter username and password (a line for each)");
            Session session = await Spotify.CreateSession(key, cache, settings, userAgent);
            session.MusicDelivered += session_MusicDeliver;

            await session.Login(Console.ReadLine(), Console.ReadLine(), false);
            session.PreferredBitrate = BitRate.Bitrate320k;

            await session.PlaylistContainer;
            while (session.PlaylistContainer.Playlists.Count < 1)
            {
                Console.WriteLine("Found {0} playlists, retrying in 2 sec.", session.PlaylistContainer.Playlists.Count);
                await Task.Delay(TimeSpan.FromSeconds(2));
            }

            //var toplist = await session.CreateToplistBrowse(ToplistType.Tracks, ToplistRegion.Everywhere, null);
            //var toptrack = await toplist.Tracks[0];
            //Console.WriteLine("Most popular track on spotify: " + toptrack.Name);
            //await session.Play(toptrack);

            //PlayQueue queue = new PlayQueue();

            var playlist = await session.PlaylistContainer.Playlists[0];
            //queue.Seed = playlist.Tracks;
            //while (!queue.IsEmpty)
            //    await session.Play(queue.Dequeue());


            Console.WriteLine("Playing random from " + playlist.Name);
            var track = await playlist.Tracks[new Random().Next(playlist.Tracks.Count)];
            Console.WriteLine("Found track " + track.Name);
            await track.Album;

            await track.Album.Browse(); // test
            await track.Artists[0].Browse(ArtistBrowseType.NoAlbums); // test

            var nPlaylist = await session.PlaylistContainer.Playlists.Create(Guid.NewGuid().ToString());
            nPlaylist.Tracks.Add(track);

            var coverId = track.Album.CoverId;
            var image = await Image.FromId(session, coverId);
            var imageData = image.GetImage();
            imageData.Save("cover.jpg");


            await session.Play(track);

            playlist = await session.Starred;
            Console.WriteLine("Playing random starred track");
            track = await playlist.Tracks[new Random().Next(playlist.Tracks.Count)];
            Console.WriteLine("Found track " + track.Name);
            await session.Play(track);

            Console.WriteLine("Enter song search");
            var search = await session.SearchTracks(Console.ReadLine(), 0, 1);
            if (search.Tracks.Count > 0)
            {
                track = await search.Tracks[0];
                Console.WriteLine("Playing " + track.Name);
                await session.Play(track);
            }
            else
            {
                Console.WriteLine("No matching tracks.");
            }
            await session.Logout();
        }

        static async Task PlayPlaylistForever(Session session, Playlist starred)
        {
            int num = 0;
            var random = new Random();
            var count = starred.Tracks.Count;
            while (true)
            {
                var track = starred.Tracks[random.Next(count)];
                Console.WriteLine(++num);
                await session.Play(track);
            }
        }

        static void session_MusicDeliver(Session sender, MusicDeliveryEventArgs e)
        {
            if (e.Samples.Length > 0)
            {
                e.ConsumedFrames = player.EnqueueSamples(e.Channels, e.Rate, e.Samples, e.Frames);
            }
            else
            {
                e.ConsumedFrames = 0;
            }
        }
    }
}

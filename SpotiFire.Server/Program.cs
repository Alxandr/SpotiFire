using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using SpotiFire.SpotifyLib;
using d = System.Drawing;

namespace SpotiFire.Server
{

    class Program
    {
        #region key

        // SpotiFire.

        static readonly byte[] key = new Byte[]
            {
                0x01, 0x16, 0xA3, 0xB3, 0x8D, 0x4C, 0x38, 0x69, 0xBE, 0xE2, 0x65, 0xD3, 0x4B, 0x57, 0x70, 0x12,
                0x6D, 0x4B, 0x28, 0x2A, 0x89, 0x7B, 0x87, 0x07, 0xC7, 0xAE, 0x68, 0xBC, 0x01, 0x20, 0xBE, 0xF9,
                0x0B, 0x65, 0xC1, 0x11, 0xA9, 0x07, 0x94, 0xEB, 0x9A, 0xCE, 0x29, 0xAF, 0x9B, 0x11, 0xE3, 0xE8,
                0x01, 0x22, 0x9B, 0x5C, 0xF5, 0x05, 0xA8, 0x91, 0xE9, 0x9F, 0xE1, 0xA9, 0x4D, 0x8C, 0x21, 0xAB,
                0x53, 0x44, 0x9E, 0xAE, 0x65, 0xA3, 0x36, 0x37, 0x6E, 0xAC, 0xFA, 0x63, 0x43, 0xEB, 0xA1, 0x89,
                0xC6, 0xAC, 0xF0, 0x79, 0x61, 0xF9, 0xB0, 0x15, 0x3B, 0x88, 0xFA, 0xA8, 0x5C, 0xC3, 0xA0, 0x74,
                0xE1, 0xC3, 0x42, 0xF3, 0x72, 0xD2, 0xD1, 0xC3, 0xD5, 0x8E, 0xE7, 0xD4, 0x8D, 0x97, 0x36, 0x54,
                0x7C, 0x58, 0x92, 0xF2, 0xE1, 0x0C, 0x40, 0x37, 0x5F, 0x6D, 0x80, 0x95, 0x58, 0x2B, 0x20, 0xC2,
                0x13, 0x5F, 0x76, 0xDB, 0x6B, 0x55, 0x8B, 0xD0, 0x0E, 0x9F, 0x3D, 0x80, 0x02, 0xC4, 0xCE, 0xF8,
                0x22, 0xE3, 0x14, 0x46, 0x4E, 0xBF, 0x37, 0xD3, 0x51, 0x9D, 0xD2, 0x42, 0x7D, 0x5A, 0xEE, 0xEC,
                0xE3, 0xA3, 0xF3, 0xBD, 0x7B, 0x77, 0x59, 0x8E, 0xD5, 0xD9, 0x7D, 0xE3, 0xCE, 0x6D, 0x15, 0x05,
                0x88, 0x3F, 0xC2, 0x27, 0x54, 0x09, 0x8C, 0x2D, 0x4D, 0x94, 0x86, 0xF0, 0x14, 0xAB, 0xA2, 0x9E,
                0xC8, 0xEF, 0xCE, 0x48, 0x48, 0xDD, 0x63, 0xAD, 0xEF, 0x40, 0x0A, 0x31, 0x81, 0xCA, 0x70, 0x89,
                0x01, 0x1A, 0x4D, 0x2B, 0xF8, 0x9F, 0x8D, 0x42, 0xB4, 0x31, 0xF1, 0xBA, 0x8A, 0x49, 0xF4, 0xFA,
                0xCD, 0x75, 0x30, 0x5F, 0x85, 0xC0, 0x0B, 0xF4, 0x27, 0x83, 0x1B, 0x34, 0x53, 0x37, 0x39, 0x35,
                0xED, 0x82, 0x73, 0xE7, 0x91, 0xA6, 0x5C, 0x85, 0x58, 0x5C, 0xC7, 0x34, 0x18, 0x3F, 0x07, 0x8C,
                0x5E, 0xF3, 0xA0, 0xC6, 0xB7, 0xC7, 0x8B, 0xF8, 0x41, 0x0D, 0x2D, 0xFD, 0x63, 0x0C, 0x6C, 0xA9,
                0xD0, 0xE7, 0x12, 0x18, 0x02, 0xB7, 0x1C, 0xFB, 0x98, 0x0D, 0xFA, 0x71, 0x98, 0xAA, 0x71, 0xDB,
                0xC8, 0x4E, 0xCB, 0x1A, 0xB2, 0xC7, 0xA1, 0x91, 0xB8, 0xD2, 0x38, 0xA7, 0x11, 0x25, 0xC6, 0xF8,
                0x3F, 0x04, 0xC4, 0x41, 0x3A, 0x40, 0x2A, 0x7D, 0xCA, 0x6C, 0xD5, 0xC1, 0x67, 0x5D, 0xA3, 0x94,
                0x1C
            };


        #endregion

        private static BASSPlayer player;
        private static string searchTerm;
        private static int currentIndex = 0;
        private static ManualResetEvent waiter = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            string username, password;

            Console.Write("Enter spotify username: ");
            username = Console.ReadLine();
            Console.Write("Enter password: ");
            password = Console.ReadLine();
            Console.Write("Enter search: ");
            searchTerm = Console.ReadLine();
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
            string dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpotiFire", "libspotify");
            try
            {
                if (!Directory.Exists(dataPath))
                    Directory.CreateDirectory(dataPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Session session = Session.Create(key, dataPath, dataPath, "SpotiFire");
            session.Exception += new SessionEventHandler(session_Exception);
            session.LogMessage += new SessionEventHandler(session_LogMessage);
            session.LoginComplete += new SessionEventHandler(session_LoginComplete);
            session.MusicDeliver += new MusicDeliveryEventHandler(session_MusicDeliver);
            session.EndOfTrack += new SessionEventHandler(session_EndOfTrack);
            session.LogoutComplete += new SessionEventHandler(session_LogoutComplete);
            session.StopPlayback += new SessionEventHandler(session_StopPlayback);
            session.MessageToUser += new SessionEventHandler(session_MessageToUser);
            session.PlayTokenLost += new SessionEventHandler(session_PlayTokenLost);
            session.Login(username.Trim(), password.Trim());
            session.SetPrefferedBitrate(sp_bitrate.BITRATE_320k);
            waiter.WaitOne();
        }

        static void session_PlayTokenLost(Session sender, SessionEventArgs e)
        {
            Console.WriteLine("PlayToken lost");
        }

        static void session_MessageToUser(Session sender, SessionEventArgs e)
        {
            Console.WriteLine("To you: " + e.Message);
        }

        static void session_StopPlayback(Session sender, SessionEventArgs e)
        {
            Console.WriteLine("Playback stoped");
        }

        static void session_LogoutComplete(Session sender, SessionEventArgs e)
        {
            Console.WriteLine("Logout complete.");
            waiter.Set();
        }

        static void session_EndOfTrack(Session sender, SessionEventArgs e)
        {
            Console.WriteLine("End of track.");

            Thread.Sleep(510); //Empty player buffer (lags for 500ms);
            player.Stop();

            DoSearch(sender);
        }

        static void session_MusicDeliver(Session sender, MusicDeliveryEventArgs e)
        {
            if (e.Samples.Length > 0)
            {
                if(player == null)
                    player = new BASSPlayer();

                e.ConsumedFrames = player.EnqueueSamples(e.Channels, e.Rate, e.Samples, e.Frames);
            }
            else
            {
                e.ConsumedFrames = 0;
            }
        }

        static void DoSearch(Session sender)
        {
            string prevSearch = searchTerm;
            Console.WriteLine("New search term? (enter \"quit\" to exit, \"next\" to play next song on previous result).");
            Console.Write("Search: ");
            searchTerm = Console.ReadLine().Trim();
            if (searchTerm != "next")
                currentIndex = 0;
            else
            {
                currentIndex++;
                searchTerm = prevSearch;
            }
            if (searchTerm == "quit")
                sender.Logout();
            else
                sender.Search(searchTerm, currentIndex, 1, 0, 0, 0, 0).Complete += new SearchEventHandler(search_Complete);
        }

        static void search_Complete(ISearch sender, EventArgs e)
        {
            Console.WriteLine("Search complete!");
            if (sender.Tracks.Length == 0)
            {
                Console.WriteLine("Did you mean \"{0}\"?", sender.DidYouMean);
                DoSearch(sender.Session);
                return;
            }
            Console.WriteLine(sender.ToString());
            ITrack trck = sender.Tracks[0];
            Console.WriteLine("Track.Name: {0}", trck.Name);
            Console.WriteLine("Album.Name: {0}", trck.Album.Name);
            foreach (var artist in trck.Artists)
                Console.WriteLine("Artist.Name: {0}", artist.Name);
            sender.Session.PlayerLoad(trck);
            sender.Session.PlayerPlay();
            IImage img = sender.Session.GetImageFromId(trck.Album.CoverId);
            if (img.IsLoaded)
                img_Loaded(img, new EventArgs());
            else
                img.Loaded += new ImageEventHandler(img_Loaded);
        }

        static void img_Loaded(IImage sender, EventArgs e)
        {
            try
            {
                d.Image img = d.Image.FromStream(new MemoryStream(sender.Data));
                sender.Loaded -= new ImageEventHandler(img_Loaded);
                string name = sender.ImageId + ".png";
                img.Save(name, d.Imaging.ImageFormat.Png);
                img.Dispose();
                var p = System.Diagnostics.Process.Start(name);
                new Thread(() =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                    try { File.Delete(name); }
                    catch { }
                    try { p.Close(); }
                    catch { }
                    sender.Dispose();
                }).Start();
            }
            catch { }
        }

        static void session_LoginComplete(Session sender, SessionEventArgs e)
        {
            Console.WriteLine("Login complete!");
            sender.Search(searchTerm, 0, 1, 0, 0, 0, 0).Complete += new SearchEventHandler(search_Complete);
            sender.PlaylistContainer.Loaded += new PlaylistContainerHandler(PlaylistContainer_Loaded);
        }

        static void PlaylistContainer_Loaded(PlaylistContainer pc, EventArgs args)
        {
            Console.WriteLine("Playlistcontainer loaded.");
        }

        static void session_LogMessage(Session sender, SessionEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        static void session_Exception(Session sender, SessionEventArgs e)
        {
            Console.WriteLine("New Exception: " + e.ToString());
        }

        static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject.ToString());
        }
    }
}

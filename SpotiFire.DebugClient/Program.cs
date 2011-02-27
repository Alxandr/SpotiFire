using System;
using System.IO;
using System.Text;
using System.Threading;
using SpotiFire.SpotifyLib;

namespace SpotiFire.DebugClient
{

    class Program
    {
        #region key

        // SpotiFire.

        static internal readonly byte[] applicationKey = new Byte[]
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

        private static ManualResetEvent waiter = new ManualResetEvent(false);

        internal static string cacheLocation;
        internal static string settingsLocation;
        internal static string userAgent = "SpotiFire";

        internal unsafe struct sp_session { }
        internal unsafe struct sp_audioformat
        {

        }
        internal enum sp_sampletype
        {
            SP_SAMPLETYPE_INT16_NATIVE_ENDIAN = 0
        }
        internal unsafe struct sp_config
        {
            sp_sampletype sample_type;
            int sample_rate;
            int channels;
        }
        internal unsafe struct sp_session_callbacks
        {
            void logged_in(sp_session* session, sp_error error);
            void logged_out(sp_session* session);
            void metadata_updated(sp_session* session);
            void connection_error(sp_session* session, sp_error error);
            void message_to_user(sp_session* session, char* message);
            void notify_main_thread(sp_session* session);
            void music_delivery(sp_session* session, sp_audioformat* format, void* frames, int num_frames);
            void play_token_lost(sp_session* session);
            void log_message(sp_session* session, char* data);
            void end_of_track(sp_session* session);
            void streaming_error(sp_session* session, sp_error error);
            void userinfo_updated(sp_session* session);
            void start_playback(sp_session* session);
            void stop_playback(sp_session* session);
            void get_audio_buffer_stats(sp_session* session);
        }

        internal unsafe struct sp_session_config
        {
            int api_version;
            char* cache_location;
            char* settings_location;
            void* application_key;
            uint application_key_size;
            char* user_agent;
            sp_session_callbacks* callbacks;
            void* userdata;
            bool compressplaylists;
            bool dont_save_metadata_for_playlists;
            bool initially_unload_playlists;
        }

        internal static extern unsafe sp_error sp_session_create(sp_session_config* config, sp_session** session);


        internal static IntPtr session = IntPtr.Zero;
        static void Main(string[] args)
        {
            cacheLocation = settingsLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpotiFire", "libspotify");
            try
            {
                if (!Directory.Exists(cacheLocation))
                    Directory.CreateDirectory(cacheLocation);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            string username;
            Console.Write("Username:");
            username = Console.ReadLine();

            string password;
            Console.Write("Password []: ");
            password = ReadPassword();



        }

        static string ReadPassword()
        {
            bool enter = false;
            StringBuilder sb = new StringBuilder();
            while (!enter)
            {
                var key = Console.ReadKey();
                var c = key.KeyChar.ToString().ToLower()[0];
                if (key.Key == ConsoleKey.Enter)
                {
                    enter = true;
                    Console.WriteLine();
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length == 0)
                        continue;
                    sb.Remove(sb.Length - 1, 1);
                    //Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    Console.Write(' ');
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    Console.Out.Flush();
                }
                else if (c >= 'a' && c <= 'z' || c >= '1' && c <= '0')
                {
                    sb.Append(key.KeyChar);
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    Console.Write('*');
                    Console.Out.Flush();
                }
                else
                {
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    Console.Write(' ');
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                }
            }
            return sb.ToString();
        }
    }
}

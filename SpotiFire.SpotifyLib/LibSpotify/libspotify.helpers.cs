using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotiFire.SpotifyLib
{
    static partial class libspotify
    {
        internal const int STRINGBUFFER_SIZE = 256;

        internal static string GetString(IntPtr ptr, string defaultValue = "")
        {
            if (ptr == IntPtr.Zero)
                return defaultValue;

            System.Collections.Generic.List<byte> l = new System.Collections.Generic.List<byte>();
            byte read = 0;
            do
            {
                read = Marshal.ReadByte(ptr, l.Count);
                l.Add(read);
            }
            while (read != 0);

            if (l.Count > 0)
                return System.Text.Encoding.UTF8.GetString(l.ToArray(), 0, l.Count - 1);
            else
                return string.Empty;
        }

        internal static string ImageIdToString(IntPtr idPtr)
        {
            if (idPtr == IntPtr.Zero)
                return string.Empty;

            byte[] id = new byte[20];
            Marshal.Copy(idPtr, id, 0, 20);

            return ImageIdToString(id);
        }

        private static string ImageIdToString(byte[] id)
        {
            if (id == null)
                return string.Empty;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (byte b in id)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }

        internal static byte[] StringToImageId(string id)
        {
            if (string.IsNullOrEmpty(id) || id.Length != 40)
                return null;
            byte[] ret = new byte[20];
            try
            {
                for (int i = 0; i < 20; i++)
                {
                    ret[i] = byte.Parse(id.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
                }
                return ret;
            }
            catch
            {
                return null;
            }
        }

        [DllImport("kernel32.dll")]
        internal static extern IntPtr LoadLibrary(String dllname);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, String procname);
    }
}

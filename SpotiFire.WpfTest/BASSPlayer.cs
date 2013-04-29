using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;

namespace SpotiFire.WpfTest
{
    class BASSPlayer
    {
        private BASSBuffer basbuffer = null;
        private STREAMPROC streamproc = null;

        public int EnqueueSamples(int channels, int rate, byte[] samples, int frames)
        {
            int consumed = 0;
            if (basbuffer == null)
            {
                Bass.BASS_Init(-1, rate, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                basbuffer = new BASSBuffer(0.5f, rate, channels, 2);
                streamproc = new STREAMPROC(Reader);
                Bass.BASS_ChannelPlay(
                    Bass.BASS_StreamCreate(rate, channels, BASSFlag.BASS_DEFAULT, streamproc, IntPtr.Zero),
                    false
                    );
            }

            if (basbuffer.Space(0) > samples.Length)
            {
                basbuffer.Write(samples, samples.Length);
                consumed = frames;
            }

            return consumed;
        }

        private int Reader(int handle, IntPtr buffer, int length, IntPtr user)
        {
            return basbuffer.Read(buffer, length, user.ToInt32());
        }

        public void Stop()
        {
            // In real world usage you must remember to free the BASS stream if not reusing it!
            basbuffer.Clear();
        }

        public float Volume
        {
            get
            {
                return Bass.BASS_GetVolume();
            }
            set
            {
                Bass.BASS_SetVolume(value);
            }
        }
    }
}

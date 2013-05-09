using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotiFire.WinFormsTest
{
    class NAudioPlayer : IPlayer
    {
        NAudio.Wave.BufferedWaveProvider buffer;

        public int EnqueueSamples(int channels, int rate, byte[] samples, int frames)
        {
            if (buffer == null)
            {
                buffer = new NAudio.Wave.BufferedWaveProvider(new NAudio.Wave.WaveFormat(rate, channels));
                NAudio.Wave.DirectSoundOut dso = new NAudio.Wave.DirectSoundOut(70);
                dso.Init(buffer);
                dso.Play();
            }
            int space = buffer.BufferLength - buffer.BufferedBytes;
            if (space > samples.Length)
            {
                buffer.AddSamples(samples, 0, samples.Length);
                return frames;
            }
            return 0;
        }

        public void Reset()
        {
            if (buffer != null)
                buffer.ClearBuffer();
        }
    }
}

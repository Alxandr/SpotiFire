using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotiFire.TestClient
{
    class NAudioPlayer : IPlayer
    {
        NAudio.Wave.BufferedWaveProvider buffer;
        NAudio.Wave.IWavePlayer player;
        int times = 0;

        public int EnqueueSamples(int channels, int rate, byte[] samples, int frames)
        {
            if (buffer == null)
            {
                buffer = new NAudio.Wave.BufferedWaveProvider(new NAudio.Wave.WaveFormat(rate, channels));
                //NAudio.Wave.DirectSoundOut dso = new NAudio.Wave.DirectSoundOut(70);
                //NAudio.Wave.AsioOut dso = new NAudio.Wave.AsioOut();
                NAudio.Wave.WaveOut dso = new NAudio.Wave.WaveOut();
                dso.Init(buffer);
                dso.Play();

                player = dso;
            }
            int space = buffer.BufferLength - buffer.BufferedBytes;
            if (space > samples.Length)
            {
                if (times == 0)
                    Console.WriteLine("Enqueue");

                times = (times + 1) % 100;
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
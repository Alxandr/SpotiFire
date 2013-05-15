using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotiFire.TestClient
{
    class NAudioWaveProvider : IWaveProvider
    {
        readonly MusicBuffer _buffer;
        public NAudioWaveProvider(MusicBuffer buffer)
        {
            _buffer = buffer;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return _buffer.Read(buffer, offset, count);
        }

        public WaveFormat WaveFormat
        {
            get { 
                int sampleRate;
                int channels;
                _buffer.GetFormat(out channels, out sampleRate);
                if (sampleRate == 0 || channels == 0)
                    return null;
                return new WaveFormat(sampleRate, channels);
            }
        }
    }
}

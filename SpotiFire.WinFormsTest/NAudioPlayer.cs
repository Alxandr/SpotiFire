using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotiFire.WinFormsTest
{
    class NAudioPlayer : IPlayer
    {
        DirectSoundOut _dso;
        SpotifyWaveProvider _wp;
        WaveFormat _wf;

        public void Reset(MusicBuffer buffer)
        {
            Debug.WriteLine("Reset");

            lock (this)
            {
                if (_wp != null)
                {
                    if (!_wf.Equals(_wp.WaveFormat))
                    {
                        _dso.Dispose();
                        _dso = null;
                    }
                }
                else
                {
                    _wp = new SpotifyWaveProvider(buffer);
                }
                _wf = _wp.WaveFormat;

                if (_dso == null)
                {
                    _dso = new DirectSoundOut(70);
                    _dso.Init(_wp);
                    _dso.Play();
                }
                else
                {
                    _dso.Stop();
                    _dso.Play();
                }
            }
        }

        class SpotifyWaveProvider : IWaveProvider
        {
            readonly MusicBuffer _buffer;
            public SpotifyWaveProvider(MusicBuffer buffer)
            {
                _buffer = buffer;
            }

            public int Read(byte[] buffer, int offset, int count)
            {
                return _buffer.Read(buffer, offset, count);
            }

            public WaveFormat WaveFormat
            {
                get
                {
                    int sampleRate;
                    int channels;
                    _buffer.GetFormat(out channels, out sampleRate);
                    return new WaveFormat(sampleRate, channels);
                }
            }
        }
    }
}

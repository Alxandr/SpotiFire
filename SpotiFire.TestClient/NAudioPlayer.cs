using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotiFire.TestClient
{
    class NAudioPlayer : IPlayer
    {
        DirectSoundOut _dso;
        NAudioWaveProvider _wp;
        WaveFormat _wf;

        public void Reset(MusicBuffer buffer)
        {
            Console.WriteLine("Reset");

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
                    _wp = new NAudioWaveProvider(buffer);
                }
                _wf = _wp.WaveFormat;

                if (_dso == null)
                {
                    _dso = new DirectSoundOut(100);
                    _dso.Init(_wp);
                    _dso.Play();
                }
            }
        }
    }
}

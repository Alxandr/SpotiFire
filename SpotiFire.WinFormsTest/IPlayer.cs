using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotiFire.WinFormsTest
{
    interface IPlayer
    {
        int EnqueueSamples(int channels, int rate, byte[] samples, int frames);
        void Reset();
    }
}

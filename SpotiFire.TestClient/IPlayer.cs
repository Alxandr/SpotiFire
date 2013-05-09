using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotiFire.TestClient
{
    interface IPlayer
    {
        int EnqueueSamples(int channels, int rate, byte[] samples, int frames);
        void Reset();
    }
}

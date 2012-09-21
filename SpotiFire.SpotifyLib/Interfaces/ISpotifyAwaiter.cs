using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotiFire.SpotifyLib
{
    public interface ISpotifyAwaiter<T> : INotifyCompletion
        where T : ISpotifyObject
    {
        bool IsCompleted { get; }
        T GetResult();
    }
}

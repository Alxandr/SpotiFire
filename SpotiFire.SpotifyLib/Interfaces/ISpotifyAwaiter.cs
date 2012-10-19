using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotiFire
{
    public interface ISpotifyAwaiter<T> : ICriticalNotifyCompletion
        where T : ISpotifyObject
    {
        bool IsCompleted { get; }
        T GetResult();
    }
}

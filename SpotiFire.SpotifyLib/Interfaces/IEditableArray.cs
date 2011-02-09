using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotiFire.SpotifyLib
{
    public interface IEditableArray<T> : IArray<T>, ICollection<T>
    {
    }
}

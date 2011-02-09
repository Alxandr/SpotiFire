using System.Collections.Generic;

namespace SpotiFire.SpotifyLib
{
    public interface IEditableArray<T> : IArray<T>, ICollection<T>
    {
        int IndexOf(T item);
    }
}

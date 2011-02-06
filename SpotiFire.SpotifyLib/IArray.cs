using System;
using System.Collections.Generic;
namespace SpotiFire.SpotifyLib
{
    public interface IArray<T> : IEnumerable<T>
    {
        int Length { get; }
        T this[int index] { get; }
    }
}

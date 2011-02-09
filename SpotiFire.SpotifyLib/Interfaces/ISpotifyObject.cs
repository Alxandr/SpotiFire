using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotiFire.SpotifyLib
{
    public interface ISpotifyObject
    {
        Session Session { get; }
    }
}

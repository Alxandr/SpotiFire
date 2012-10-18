﻿using System;
using System.Runtime.CompilerServices;
namespace SpotiFire.SpotifyLib
{
    public interface ISearch : ISpotifyObject, IDisposable
    {
        IArray<IAlbum> Albums { get; }
        IArray<IArtist> Artists { get; }
        event SearchEventHandler Complete;
        bool IsComplete { get; }
        string DidYouMean { get; }
        Error Error { get; }
        string Query { get; }
        int TotalAlbums { get; }
        int TotalArtists { get; }
        int TotalTracks { get; }
        IArray<ITrack> Tracks { get; }
    }
}

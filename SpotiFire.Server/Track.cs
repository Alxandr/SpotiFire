using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using SpotiFire.SpotifyLib;

namespace SpotiFire.Server
{
    [DataContract, DebuggerDisplay("Track - {Name}")]
    public class Track
    {
        public Track(ITrack track)
        {
            Name = track.Name;
            Artists = track.Artists.Select(a => new Artist(a)).ToArray();
            Album = track.Album.Name;
            Length = track.Duration;
            IsAvailable = track.IsAvailable;
            Popularity = track.Popularity;
            IsStarred = track.IsStarred;
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Artist[] Artists { get; set; }

        [DataMember]
        public string Album { get; set; }

        [DataMember]
        public TimeSpan Length { get; set; }

        [DataMember]
        public bool IsAvailable { get; set; }

        [DataMember]
        public int Popularity { get; set; }

        [DataMember]
        public bool IsStarred { get; set; }
    }
}

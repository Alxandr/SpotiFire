using System.Linq;
using System.Runtime.Serialization;
using SpotiFire.SpotifyLib;

namespace SpotiFire.Server
{
    [DataContract]
    public class Track
    {
        public Track(ITrack track)
        {
            Name = track.Name;
            Artists = track.Artists.Select(a => a.Name).ToArray();
            Album = track.Album.Name;
            Length = track.Duration;
            IsAvailable = track.IsAvailable;
            Popularity = track.Popularity;
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string[] Artists { get; set; }

        [DataMember]
        public string Album { get; set; }

        [DataMember]
        public int Length { get; set; }

        [DataMember]
        public bool IsAvailable { get; set; }

        [DataMember]
        public int Popularity { get; set; }
    }
}

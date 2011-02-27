
using System.Runtime.Serialization;
using SpotiFire.SpotifyLib;
namespace SpotiFire.Server
{
    [DataContract]
    public class Artist
    {
        public Artist(IArtist artist)
        {
            this.Name = artist.Name;
            this.Link = artist.CreateLink().ToString();
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Link { get; set; }
    }
}

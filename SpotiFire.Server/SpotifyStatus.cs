using System;
using System.Runtime.Serialization;

namespace SpotiFire.Server
{
    [DataContract]
    public class SpotifyStatus
    {
        [DataMember]
        public bool Shuffle { get; set; }

        [DataMember]
        public bool Repeat { get; set; }

        [DataMember]
        public Track CurrentTrack { get; set; }

        [DataMember]
        public TimeSpan LengthPlayed { get; set; }

        [DataMember]
        public bool IsPlaying { get; set; }

        [DataMember]
        public bool CanStartPlayback { get; set; }

        [DataMember]
        public bool CanGoBack { get; set; }

        [DataMember]
        public bool CanGoNext { get; set; }

        [DataMember]
        public int Volume { get; set; }
    }
}

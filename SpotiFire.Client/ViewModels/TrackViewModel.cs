using System;
using System.ComponentModel;

namespace SpotiFire.SpotiClient.ViewModels
{
    public class TrackViewModel : BaseViewModel
    {
        #region Data
        private String name;
        private String album;
        private TimeSpan length;
        private bool isStarred;
        private bool isAvailable;
        private BindingList<ArtistViewModel> artists;
        #endregion

        #region Constructors
        public TrackViewModel(ServiceReference.Track track)
        {
            length = track.Length;
            name = track.Name;
            album = track.Album;
            isStarred = track.IsStarred;
            isAvailable = track.IsAvailable;
            artists = new BindingList<ArtistViewModel>();
            foreach (var artist in track.Artists)
                artists.Add(new ArtistViewModel(artist));
        }
        #endregion

        #region Properties
        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                if (!Object.ReferenceEquals(name, value))
                {
                    name = value;
                    OnPropertyChanged(() => Name);
                }
            }
        }

        public TimeSpan Length
        {
            get
            {
                return length;
            }
            set
            {
                if (value != length)
                {
                    length = value;
                    OnPropertyChanged(() => Length);
                }
            }
        }

        public bool IsStarred
        {
            get
            {
                return isStarred;
            }
            set
            {
                if (value != isStarred)
                {
                    isStarred = value;
                    OnPropertyChanged(() => IsStarred);
                }
            }
        }

        public bool IsAvailable
        {
            get
            {
                return isAvailable;
            }
            set
            {
                if (value != isAvailable)
                {
                    isAvailable = value;
                    OnPropertyChanged(() => IsAvailable);
                }
            }
        }

        public string Album
        {
            get
            {
                return album;
            }
            set
            {
                if (!Object.ReferenceEquals(album, value))
                {
                    album = value;
                    OnPropertyChanged(() => Album);
                }
            }
        }

        public BindingList<ArtistViewModel> Artists
        {
            get
            {
                return artists;
            }
        }
        #endregion
    }
}

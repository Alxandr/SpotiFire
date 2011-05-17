
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;
namespace SpotiFire.SpotiClient.ViewModels
{
    public class PlaylistViewModel : BaseViewModel, ISpotifyViewViewModel
    {
        #region Data
        private Guid id;
        private string name;
        private BindingList<TrackViewModel> tracks;
        private bool loaded;
        #endregion

        #region View
        private int tabIndex = 0;
        public IEnumerable<TabItem> Tabs
        {
            get
            {
                yield return new TabItem()
                {
                    Header = Name,
                    Content = new SpotifyPlaylistView()
                    {
                        DataContext = this
                    }
                };
            }
        }

        public int SelectedTabIndex
        {
            get
            {
                return tabIndex;
            }
            set
            {
                if (value != tabIndex)
                {
                    tabIndex = value;
                    OnPropertyChanged(() => SelectedTabIndex);
                }
            }
        }
        #endregion

        #region Constructors
        public PlaylistViewModel(ServiceReference.Playlist pl)
        {
            Id = pl.Id;
            Name = pl.Name;
            tracks = new BindingList<TrackViewModel>();
            loaded = false;
        }
        #endregion

        #region Properties
        public Guid Id
        {
            get
            {
                return id;
            }
            private set
            {
                if (value != id)
                {
                    id = value;
                    OnPropertyChanged(() => Id);
                }
            }
        }

        public string Name
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

        public BindingList<TrackViewModel> Tracks
        {
            get
            {
                if (!loaded)
                {
                    loaded = true;
                    ThreadPool.QueueUserWorkItem((o) =>
                    {
                        List<TrackViewModel> midTracks = new List<TrackViewModel>();
                        foreach (var t in SpotifyViewModel.Instance.GetPlaylistTracks(id))
                        {
                            midTracks.Add(new TrackViewModel(t));
                        }
                        App.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            foreach (var t in midTracks)
                                tracks.Add(t);
                        }));
                    });
                }
                return tracks;
            }
        }
        #endregion
    }
}

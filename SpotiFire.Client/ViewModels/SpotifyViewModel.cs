using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SpotiFire.SpotiClient.ServiceReference;

namespace SpotiFire.SpotiClient.ViewModels
{
    public class SpotifyViewModel : BaseViewModel, ServiceReference.SpotifyCallback
    {
        #region SpotifyClient
        private ServiceReference.Spotify client;
        #endregion

        #region Data
        // Current playback
        private bool isPlaying;
        private bool canStartPlayback;
        private bool canGoBack;
        private bool canGoNext;

        // Current track
        private TrackViewModel track;
        private TimeSpan currentTime;
        private DateTime lastQueried;

        // Playback list order
        private bool shuffle;
        private bool repeat;

        // Player values
        private int volume;

        // Playlist-tree
        private ReadOnlyCollection<PlaylistTreeItemViewModel> playlists;
        #endregion

        #region Constructors
        public SpotifyViewModel()
        {
            shuffle = false;
            repeat = false;

            isPlaying = false;
            canStartPlayback = false;
            canGoBack = false;
            canGoNext = false;

            volume = 100;

            playlists = null;
        }
        #endregion

        #region Spotify and client helper methods
        public ServiceReference.Spotify Client
        {
            set
            {
                if (client == null)
                {
                    client = value;
                    ApplyStatus(client.GetStatus());
                    ApplyPlaylists(client.GetPlaylists());
                }
            }
        }

        private IEnumerable<PlaylistTreeItemViewModel> MakePlaylistTree(Playlist[] playlists)
        {
            for (int i = 0; i < playlists.Length; i++)
            {
                if (playlists[i].Type == PlaylistType.Playlist)
                    yield return PlaylistTreeItemViewModel.FromSPPlaylist(playlists[i]);

                else if (playlists[i].Type == PlaylistType.FolderStart)
                {
                    Playlist p = playlists[i];
                    int depth = 1;
                    IList<Playlist> intermedium = new List<Playlist>();
                    for (i++; i < playlists.Length; i++)
                    {
                        if (playlists[i].Type == PlaylistType.Playlist)
                            intermedium.Add(playlists[i]);
                        else if (playlists[i].Type == PlaylistType.FolderStart)
                        {
                            intermedium.Add(playlists[i]);
                            depth++;
                        }
                        else if (playlists[i].Type == PlaylistType.FolderEnd)
                        {
                            if (--depth == 0)
                            {
                                yield return PlaylistTreeItemViewModel.FromSPPlaylist(p, MakePlaylistTree(intermedium.ToArray()).ToArray());
                                break;
                            }
                            else
                            {
                                intermedium.Add(playlists[i]);
                            }
                        }
                    }
                }
            }
            yield break;
        }

        private void ApplyPlaylists(ServiceReference.Playlist[] playlist)
        {
            Playlists = new ReadOnlyCollection<PlaylistTreeItemViewModel>(MakePlaylistTree(playlist).ToArray());
        }

        private void ApplyStatus(ServiceReference.SpotifyStatus spotifyStatus)
        {
            Shuffle = spotifyStatus.Shuffle;
            Repeat = spotifyStatus.Repeat;

            IsPlaying = spotifyStatus.IsPlaying;
            CanStartPlayback = spotifyStatus.CanStartPlayback;
            CanGoBack = spotifyStatus.CanGoBack;
            CanGoNext = spotifyStatus.CanGoNext;

            Volume = spotifyStatus.Volume;
        }
        #endregion

        #region Properties
        public bool Shuffle
        {
            get
            {
                return shuffle;
            }
            set
            {
                if (value != shuffle)
                {
                    shuffle = value;
                    OnPropertyChanged(() => Shuffle);
                }
            }
        }

        public bool Repeat
        {
            get
            {
                return repeat;
            }
            set
            {
                if (value != repeat)
                {
                    repeat = value;
                    OnPropertyChanged(() => Repeat);
                }
            }
        }

        public bool IsPlaying
        {
            get
            {
                return isPlaying;
            }
            set
            {
                if (value != isPlaying)
                {
                    isPlaying = value;
                    OnPropertyChanged(() => IsPlaying);
                }
            }
        }

        public bool CanStartPlayback
        {
            get
            {
                return canStartPlayback;
            }
            set
            {
                if (value != canStartPlayback)
                {
                    canStartPlayback = value;
                    OnPropertyChanged(() => CanStartPlayback);
                }
            }
        }

        public bool CanGoNext
        {
            get
            {
                return canGoNext;
            }
            set
            {
                if (value != canGoNext)
                {
                    canGoNext = value;
                    OnPropertyChanged(() => CanGoNext);
                }
            }
        }

        public bool CanGoBack
        {
            get
            {
                return canGoBack;
            }
            set
            {
                if (value != canGoBack)
                {
                    canGoBack = value;
                    OnPropertyChanged(() => CanGoBack);
                }
            }
        }

        public int Volume
        {
            get
            {
                return volume;
            }
            set
            {
                if (value != volume)
                {
                    volume = value;
                    OnPropertyChanged(() => Volume);
                }
            }
        }

        public ReadOnlyCollection<PlaylistTreeItemViewModel> Playlists
        {
            get
            {
                return playlists;
            }
            private set
            {
                if (!Object.ReferenceEquals(playlists, value))
                {
                    playlists = value;
                    OnPropertyChanged(() => Playlists);
                }
            }
        }
        #endregion

        #region Spotire Callbacks
        public void Ping()
        {
            client.Pong();
        }

        public void SongStarted(ServiceReference.Track track, Guid containerId, int index)
        {
            //x throw new NotImplementedException();
        }

        public void PlaybackStarted()
        {
            //x throw new NotImplementedException();
        }

        public void PlaybackEnded()
        {
            //x throw new NotImplementedException();
        }
        #endregion
    }
}

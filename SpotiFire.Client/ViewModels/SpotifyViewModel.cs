using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using SpotiFire.SpotiClient.Commands;
using SpotiFire.SpotiClient.ServiceReference;

namespace SpotiFire.SpotiClient.ViewModels
{
    public sealed class SpotifyViewModel : BaseViewModel, ServiceReference.SpotifyCallback
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
        BindingList<PlaylistTreeItemViewModel> playlists;

        // Viewobject
        private ISpotifyViewViewModel currentView;
        #endregion

        #region Command Instances
        private ICommand playPauseCommand;
        #endregion

        #region Command Properties
        public ICommand PlayPauseCommand
        {
            get { return playPauseCommand; }
        }
        #endregion

        #region Command Implementations
        private void PlayPause(object parameter)
        {
            if (track == null)
            {
                client.PlayPlaylistTrack(currentView.Id, -1);
            }
            else
            {
                client.PlayPause();
            }
        }
        #endregion

        #region Singelton
        public static SpotifyViewModel Instance
        {
            get
            {
                return viewModel;
            }
        }
        #endregion

        #region Constructors
        private static SpotifyViewModel viewModel;
        static SpotifyViewModel()
        {
            viewModel = new SpotifyViewModel();
        }
        private SpotifyViewModel()
        {
            playPauseCommand = new PropertyBoundCommand(new CommandExecute(PlayPause), this, "CanStartPlayback");

            shuffle = false;
            repeat = false;

            isPlaying = false;
            canStartPlayback = false;
            canGoBack = false;
            canGoNext = false;

            volume = 100;

            playlists = new BindingList<PlaylistTreeItemViewModel>();
            playlists.ListChanged += new ListChangedEventHandler(playlists_ListChanged);
        }
        #endregion

        #region Event listeners
        #region Helper
        private IEnumerable<PlaylistTreeItemViewModel> FlattenPlaylistTree(IEnumerable<PlaylistTreeItemViewModel> playlists = null)
        {
            if (playlists == null)
                playlists = this.playlists;
            Queue<PlaylistTreeItemViewModel.PlaylistFolder> folders = new Queue<PlaylistTreeItemViewModel.PlaylistFolder>();
            foreach (var p in playlists)
            {
                yield return p;
                if (p is PlaylistTreeItemViewModel.PlaylistFolder)
                    folders.Enqueue((PlaylistTreeItemViewModel.PlaylistFolder)p);
            }

            while (folders.Count > 0)
                foreach (var p in FlattenPlaylistTree(folders.Dequeue().Children))
                    yield return p;

            yield break;
        }
        #endregion

        void playlists_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemChanged && !(currentView is PlaylistTreeItemViewModel && ((PlaylistTreeItemViewModel)currentView).IsSelected))
            {
                var newView = FlattenPlaylistTree().Where(p => p.IsSelected && !(p is PlaylistTreeItemViewModel.PlaylistFolder)).FirstOrDefault();
                if (newView != null)
                    CurrentView = newView;
            }
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

        public Track[] GetPlaylistTracks(Guid id)
        {
            return client.GetPlaylistTracks(id);
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
            foreach (var p in MakePlaylistTree(playlist))
                playlists.Add(p);
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

            Track = spotifyStatus.CurrentTrack != null ? new TrackViewModel(spotifyStatus.CurrentTrack) : null;
            currentTime = spotifyStatus.LengthPlayed;
            lastQueried = DateTime.Now;
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

        public BindingList<PlaylistTreeItemViewModel> Playlists
        {
            get
            {
                return playlists;
            }
        }

        public ISpotifyViewViewModel CurrentView
        {
            get
            {
                return currentView;
            }
            set
            {
                if (!Object.ReferenceEquals(currentView, value))
                {
                    if (value is PlaylistViewModel)
                        CanStartPlayback = true;
                    currentView = value;
                    OnPropertyChanged(() => CurrentView);
                }
            }
        }

        public TrackViewModel Track
        {
            get
            {
                return track;
            }
            set
            {
                if (!Object.ReferenceEquals(track, value))
                {
                    track = value;
                    currentTime = TimeSpan.Zero;
                    OnPropertyChanged(() => Track);
                }
            }
        }

        public TimeSpan CurrentTime
        {
            get
            {
                return currentTime;
            }
            set
            {
                if (currentTime != value)
                {
                    lastQueried = DateTime.Now;
                    OnPropertyChanged(() => CurrentTime);
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

        public void UpdateTrackTime()
        {
            if (lastQueried == DateTime.MinValue || DateTime.Now == lastQueried || track == null || currentTime > track.Length)
                return;

            DateTime dt = DateTime.Now;
            currentTime += dt.Subtract(lastQueried);
            lastQueried = dt;
            OnPropertyChanged(() => CurrentTime);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SpotiFire.SpotiClient.ViewModels
{
    public abstract class PlaylistTreeItemViewModel : PlaylistViewModel
    {
        #region Data
        private PlaylistTreeItemViewModel parent;
        private bool isSelected;
        #endregion

        #region Static helpers
        public static PlaylistTreeItemViewModel FromSPPlaylist(ServiceReference.Playlist playlist, IEnumerable<PlaylistTreeItemViewModel> children = null)
        {
            if (playlist.Type == ServiceReference.PlaylistType.Playlist)
                return new Playlist(playlist);
            else
                return new PlaylistFolder(playlist, children);
        }
        #endregion

        #region Properties
        public PlaylistTreeItemViewModel Parent
        {
            get
            {
                return parent;
            }
        }

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (value != isSelected)
                {
                    isSelected = value;
                    OnPropertyChanged(() => IsSelected);
                }
            }
        }

        public abstract string[] IconPaths { get; }
        public abstract bool IsExpanded { get; set; }
        #endregion

        #region Constructors
        public PlaylistTreeItemViewModel(ServiceReference.Playlist pl)
            : base(pl)
        {
            parent = null;
        }
        #endregion

        #region Implementations
        public class Playlist : PlaylistTreeItemViewModel
        {
            #region IconData
            private static readonly string[] iconPaths = new string[] {
                "Images/playlist_icon.png",
                "Images/playlist_icon_selected.png"
            };
            #endregion

            #region Constructors
            public Playlist(ServiceReference.Playlist pl)
                : base(pl)
            { }
            #endregion

            #region Properties
            public override string[] IconPaths
            {
                get { return iconPaths; }
            }

            public override bool IsExpanded
            {
                get
                {
                    return false;
                }
                set
                {
                    if (value)
                        throw new InvalidOperationException();
                }
            }
            #endregion

            #region ToStrings
            public override string ToString()
            {
                return "Playlist " + Name;
            }
            #endregion
        }

        public class PlaylistFolder : PlaylistTreeItemViewModel
        {
            #region IconData
            private static readonly string[] iconPaths = new string[] {
                "Images/playlist_folder_icon.png",
                "Images/playlist_folder_icon_selected.png"
            };
            #endregion

            #region Data
            private BindingList<PlaylistTreeItemViewModel> children;
            private bool isExpanded;
            #endregion

            #region Constructors
            public PlaylistFolder(ServiceReference.Playlist pl, IEnumerable<PlaylistTreeItemViewModel> children)
                : base(pl)
            {
                if (children == null)
                    this.children = new BindingList<PlaylistTreeItemViewModel>();
                else
                {
                    var list = new List<PlaylistTreeItemViewModel>();
                    foreach (var p in children)
                    {
                        list.Add(p);
                        parent = this;
                    }
                    this.children = new BindingList<PlaylistTreeItemViewModel>(list);
                }

                this.children.ListChanged += (s, e) => OnPropertyChanged(() => Children);
            }
            #endregion

            #region Properties
            public override string[] IconPaths
            {
                get { return iconPaths; }
            }

            public BindingList<PlaylistTreeItemViewModel> Children
            {
                get
                {
                    return children;
                }
            }

            public override bool IsExpanded
            {
                get
                {
                    return isExpanded;
                }
                set
                {
                    if (value != isExpanded)
                    {
                        isExpanded = value;
                        OnPropertyChanged(() => IsExpanded);

                        if (isExpanded && Parent != null)
                            Parent.IsExpanded = true;
                    }
                }
            }
            #endregion

            #region ToStrings
            public override string ToString()
            {
                return "PlaylistFolder " + Name + " (" + Children.Count + ")";
            }
            #endregion
        }
        #endregion
    }
}

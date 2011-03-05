using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SpotiFire.SpotiClient.ViewModels
{
    public abstract class PlaylistTreeItemViewModel : PlaylistViewModel
    {
        #region Data
        private PlaylistTreeItemViewModel parent;
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
            #region Constructors
            public Playlist(ServiceReference.Playlist pl)
                : base(pl)
            { }
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
            #region Data
            private ReadOnlyCollection<PlaylistTreeItemViewModel> children;
            #endregion

            #region Constructors
            public PlaylistFolder(ServiceReference.Playlist pl, IEnumerable<PlaylistTreeItemViewModel> children)
                : base(pl)
            {
                if (children == null)
                    this.children = new ReadOnlyCollection<PlaylistTreeItemViewModel>(new List<PlaylistTreeItemViewModel>());
                else
                {
                    var list = new List<PlaylistTreeItemViewModel>();
                    foreach (var p in children)
                    {
                        list.Add(p);
                        parent = this;
                    }
                    this.children = new ReadOnlyCollection<PlaylistTreeItemViewModel>(list);
                }
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

            public ReadOnlyCollection<PlaylistTreeItemViewModel> Children
            {
                get
                {
                    return children;
                }
            }
            #endregion

            #region ToStrings
            public override string ToString()
            {
                return string.Format("{PlaylistFolder Name={0}, Count={1}}", Name, Children.Count);
            }
            #endregion
        }
        #endregion
    }
}


using System;
namespace SpotiFire.SpotiClient.ViewModels
{
    public class PlaylistViewModel : BaseViewModel
    {
        #region Data
        private Guid id;
        private string name;
        #endregion

        #region Constructors
        public PlaylistViewModel(ServiceReference.Playlist pl)
        {
            Id = pl.Id;
            Name = pl.Name;
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
        #endregion
    }
}

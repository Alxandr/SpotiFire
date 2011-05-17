using SpotiFire.SpotiClient.ServiceReference;

namespace SpotiFire.SpotiClient.ViewModels
{
    public class ArtistViewModel : BaseViewModel
    {
        #region Data
        private string name;
        private string link;
        #endregion

        #region Constructors
        public ArtistViewModel(Artist artist)
        {
            name = artist.Name;
            link = artist.Link;
        }
        #endregion

        #region Properties
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (!object.ReferenceEquals(name, value))
                {
                    name = value;
                    OnPropertyChanged(() => Name);
                }
            }
        }

        public string Link
        {
            get
            {
                return link;
            }
            set
            {
                if (!object.ReferenceEquals(link, value))
                {
                    name = value;
                    OnPropertyChanged(() => Link);
                }
            }
        }
        #endregion
    }
}

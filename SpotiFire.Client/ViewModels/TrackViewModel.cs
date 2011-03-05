using System;

namespace SpotiFire.SpotiClient.ViewModels
{
    public class TrackViewModel : BaseViewModel
    {
        #region Data
        private String name;
        private TimeSpan length;
        #endregion

        #region Constructors
        public TrackViewModel()
        {
            length = TimeSpan.Zero;
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
        #endregion
    }
}

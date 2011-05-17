using System;
using System.ComponentModel;
using System.Windows.Input;
using SpotiFire.SpotiClient.ViewModels;

namespace SpotiFire.SpotiClient.Commands
{
    public class PlayPauseCommand : ICommand
    {
        private SpotifyViewModel model;

        public PlayPauseCommand(SpotifyViewModel model)
        {
            this.model = model;
            this.model.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);
        }

        void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CanStartPlayback")
                if (CanExecuteChanged != null)
                    CanExecuteChanged(this, e);
        }

        public bool CanExecute(object parameter)
        {
            return model.CanStartPlayback;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}

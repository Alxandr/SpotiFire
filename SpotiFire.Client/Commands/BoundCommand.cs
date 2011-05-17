using System;
using System.ComponentModel;
using System.Windows.Input;

namespace SpotiFire.SpotiClient.Commands
{
    public delegate void CommandExecute(object parameter);
    public delegate bool CommandCanExecute(object parameter);
    public class BoundCommand : ICommand
    {
        private CommandExecute execute;
        private CommandCanExecute canExecute;
        private INotifyPropertyChanged canExecuteChangedListener;
        private string propertyName;

        public BoundCommand(CommandExecute execute, CommandCanExecute canExecute, INotifyPropertyChanged propertyChanged = null, string propertyName = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
            if (String.IsNullOrWhiteSpace(propertyName))
                propertyChanged = null;

            if (propertyChanged != null)
            {
                propertyChanged.PropertyChanged += new PropertyChangedEventHandler(propertyChanged_PropertyChanged);
                this.propertyName = propertyName;
                this.canExecuteChangedListener = propertyChanged;
            }
        }

        void propertyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == canExecuteChangedListener && e.PropertyName == propertyName)
                if (CanExecuteChanged != null)
                    CanExecuteChanged(this, e);
        }

        public bool CanExecute(object parameter)
        {
            return canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}

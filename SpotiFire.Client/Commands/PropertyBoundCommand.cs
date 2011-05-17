using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;

namespace SpotiFire.SpotiClient.Commands
{
    public class PropertyBoundCommand : ICommand
    {
        private CommandExecute execute;
        private INotifyPropertyChanged boundTo;
        private string propertyName;
        private PropertyInfo property;
        private MethodInfo propertyGetMethod;

        public PropertyBoundCommand(CommandExecute execute, INotifyPropertyChanged boundTo, string propertyName)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            this.execute = execute;

            if (boundTo == null)
                throw new ArgumentNullException("boundTo");
            this.boundTo = boundTo;

            property = boundTo.GetType().GetProperty(propertyName);
            if (property == null)
                throw new ArgumentException("Bad propertyName");

            if (property.PropertyType != typeof(bool))
                throw new ArgumentException("Bad property type");

            propertyGetMethod = property.GetGetMethod();
            if (propertyGetMethod == null)
                throw new ArgumentException("No public get-method found.");

            this.propertyName = propertyName;

            boundTo.PropertyChanged += new PropertyChangedEventHandler(boundTo_PropertyChanged);
        }

        void boundTo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == boundTo && e.PropertyName == propertyName)
                if (CanExecuteChanged != null)
                    CanExecuteChanged(this, e);
        }

        public bool CanExecute(object parameter)
        {
            return (bool)propertyGetMethod.Invoke(boundTo, null);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}

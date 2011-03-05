using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace SpotiFire.SpotiClient.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        private string NameOf<T>(Expression<Func<T>> expr)
        {
            return ((MemberExpression)expr.Body).Member.Name;
        }
        protected void OnPropertyChanged<T>(Expression<Func<T>> expr)
        {
            OnPropertyChanged(NameOf(expr));
        }
        protected void OnPropertyChanged(String propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
        protected void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, args);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}

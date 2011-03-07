using System;
using System.ComponentModel;
using System.Windows;

namespace SpotiFire.SpotiClient.ViewModels
{
    public interface ISpotifyViewViewModel : INotifyPropertyChanged
    {
        Guid Id { get; }
        FrameworkElement GetView();
    }
}

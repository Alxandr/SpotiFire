using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

namespace SpotiFire.SpotiClient.ViewModels
{
    public interface ISpotifyViewViewModel : INotifyPropertyChanged
    {
        Guid Id { get; }
        IEnumerable<TabItem> Tabs { get; }
        int SelectedTabIndex { get; set; }
    }
}

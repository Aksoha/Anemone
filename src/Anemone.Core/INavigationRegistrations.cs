using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Anemone.Core;

public interface INavigationRegistrations : INotifyPropertyChanged, INotifyCollectionChanged,
    IEnumerable<NavigationPanelItem>
{
    void Register(NavigationPanelItem panelItem);
}
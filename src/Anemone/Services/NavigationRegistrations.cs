using System.Collections.ObjectModel;
using Anemone.Core;

namespace Anemone.Services;

public class NavigationRegistrations : ObservableCollection<NavigationPanelItem>, INavigationRegistrations
{
    public void Register(NavigationPanelItem panelItem)
    {
        Add(panelItem);
    }
}
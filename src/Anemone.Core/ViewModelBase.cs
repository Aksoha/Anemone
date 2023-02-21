using System;
using Prism.Mvvm;
using Prism.Regions;

namespace Anemone.Core;

public class ViewModelBase : BindableBase, IConfirmNavigationRequest
{
    public void OnNavigatedTo(NavigationContext navigationContext)
    {

    }

    public bool IsNavigationTarget(NavigationContext navigationContext)
    {
        return true;
    }

    public void OnNavigatedFrom(NavigationContext navigationContext)
    {

    }

    public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
    {
        continuationCallback(true);
    }
}
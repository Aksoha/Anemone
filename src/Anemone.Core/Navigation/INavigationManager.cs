using System;

namespace Anemone.Core.Navigation;

public interface INavigationManager
{
    public void Subscribe(Action<NavigationContext> callback);
    public void Unsubscribe(Action<NavigationContext> callback);
    void Navigate(string region, string uri);
}
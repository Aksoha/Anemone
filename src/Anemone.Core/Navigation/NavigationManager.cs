using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Prism.Regions;

namespace Anemone.Core.Navigation;

public class NavigationManager : INavigationManager
{
    private readonly List<Action<NavigationContext>> _callbacks = new();

    public NavigationManager(IRegionManager regionManager, ILogger<NavigationManager> logger)
    {
        RegionManager = regionManager;
        Logger = logger;
    }

    private IRegionManager RegionManager { get; }
    private ILogger<NavigationManager> Logger { get; }


    public void Subscribe(Action<NavigationContext> callback)
    {
        _callbacks.Add(callback);
    }

    public void Unsubscribe(Action<NavigationContext> callback)
    {
        _callbacks.Remove(callback);
    }

    public void Navigate(string region, string uri)
    {
        if (InvokeCallbacks(region, uri)) return;
        Navigate_Internal(region, uri);
    }

    private bool InvokeCallbacks(string region, string uri)
    {
        var context = new NavigationContext(region, uri);
        foreach (var callback in _callbacks)
        {
            callback(context);
            if (!context.IsHandled) continue;
            Logger.LogDebug("{Region} region navigation to {Uri} was handled by the {Method}", region, uri,
                callback.Method.Name);
            return true;
        }

        return false;
    }

    private void Navigate_Internal(string regionName, string uri)
    {
        RegionManager.RequestNavigate(regionName, uri, result =>
        {
            if (result.Result is false)
                LogNavigationError(result);
        });
        Logger.LogDebug("navigated to {Uri} in {Region} region", uri, regionName);
    }

    private void LogNavigationError(NavigationResult result)
    {
        Logger.LogError(result.Error, "unhandled exception has occured while invoking navigation to the {Path}",
            result.Context.Uri);
    }
}
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Prism.Regions;

namespace Anemone.Core.Navigation.Regions;

public class RegionCollection : IRegionCollection
{
    private readonly List<Action<RegionCollectionContext>> _actionCallback = new();

    public RegionCollection(IRegionManager regionManager, ILogger<RegionCollection> logger)
    {
        RegionManager = regionManager;
        Logger = logger;
    }

    private IRegionManager RegionManager { get; }
    private ILogger<RegionCollection> Logger { get; }

    public void Subscribe(Action<RegionCollectionContext> callback)
    {
        _actionCallback.Add(callback);
    }

    public void Unsubscribe(Action<RegionCollectionContext> callback)
    {
        _actionCallback.Remove(callback);
    }

    public void Add(string region, Type viewModel)
    {
        if (InvokeCallbackHandlers(region, null, viewModel, RegionCollectionAction.Add)) return;
        Add_Internal(region, viewModel);
    }

    public void Add(string region, object view)
    {
        if (InvokeCallbackHandlers(region, view, null, RegionCollectionAction.Add)) return;
        Add_Internal(region, view);
    }

    public void Remove(string region, Type viewModel)
    {
        if (InvokeCallbackHandlers(region, null, viewModel, RegionCollectionAction.Add)) return;
        Remove_Internal(region, viewModel);
    }

    public void Remove(string region, object view)
    {
        if (InvokeCallbackHandlers(region, view, null, RegionCollectionAction.Add)) return;
        Remove_Internal(region, view);
    }

    private void Add_Internal(string region, object obj)
    {
        RegionManager.Regions[region].Add(obj);
        Logger.LogDebug("{Type} was added to the {Region} region", obj.GetType(), region);
    }

    private void Remove_Internal(string region, object obj)
    {
        RegionManager.Regions[region].Remove(obj);
        Logger.LogDebug("{Type} was removed from the {Region} region", obj.GetType(), region);
    }

    private bool InvokeCallbackHandlers(string region, object? instance, Type? type, RegionCollectionAction action)
    {
        var context = new RegionCollectionContext(region, instance, type, action);
        foreach (var callback in _actionCallback)
        {
            callback(context);
            if (!context.IsHandled) continue;
            LogHandledInvocation(region, instance, type, action, callback);
            return true;
        }

        return false;
    }

    private void LogHandledInvocation(string region, object? instance, Type? type, RegionCollectionAction action,
        Action<RegionCollectionContext> callback)
    {
        var handledType = instance is null ? type : instance.GetType();

        Logger.LogDebug(
            "{Region} region action {Action} for type {Type} was handled by the {Method} of {DeclaringType}",
            region, action, handledType, callback.Method.ToString(), callback.Method.DeclaringType);
    }
}
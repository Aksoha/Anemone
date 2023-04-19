using System;

namespace Anemone.Core.Navigation.Regions;

public class RegionCollectionContext
{
    public RegionCollectionContext(string region, object? instance, Type? type, RegionCollectionAction action)
    {
        Region = region;
        Instance = instance;
        Type = type;
        Action = action;
    }

    public string Region { get; }
    public object? Instance { get; }
    public Type? Type { get; }
    public RegionCollectionAction Action { get; }
    public bool IsHandled { get; set; }
}
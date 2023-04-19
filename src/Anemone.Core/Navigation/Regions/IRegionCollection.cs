using System;

namespace Anemone.Core.Navigation.Regions;

public interface IRegionCollection
{
    void Subscribe(Action<RegionCollectionContext> callback);
    void Unsubscribe(Action<RegionCollectionContext> callback);

    void Add(string region, Type viewModel);
    void Add(string region, object view);

    void Remove(string region, Type viewModel);
    void Remove(string region, object view);
}
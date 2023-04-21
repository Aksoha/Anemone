using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Anemone.Core;
using Anemone.Models;
using Anemone.Settings;
using Anemone.UI;
using Anemone.UI.Core;
using Anemone.UI.Core.Navigation;
using Anemone.UI.Core.Navigation.Regions;

namespace Anemone.ViewModels;

public class SidebarViewModel : ViewModelBase, IDisposable
{
    private SidebarElement? _selectedItem;

    public SidebarViewModel(SidebarSettings settings, INavigationManager navigationManager,
        IRegionCollection regionCollection)
    {
        Settings = settings;
        NavigationManager = navigationManager;
        RegionCollection = regionCollection;
        CurrentWidth = Settings.IsExpanded ? Settings.MaxWidth : Settings.MinWidth;
        NavigationManager.Subscribe(NavigationHandler);
        RegionCollection.Subscribe(RegionCollectionChangedHandler);
    }

    public SidebarSettings Settings { get; }
    private INavigationManager NavigationManager { get; }
    private IRegionCollection RegionCollection { get; }

    public ObservableCollection<SidebarElement> ItemsSource { get; } = new();

    public SidebarElement? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (!SetProperty(ref _selectedItem, value)) return;
            if(_updatingSelectedItem is false)
                InvokeNavigation();
        }
    }

    /// <summary>
    /// prevents doubled navigation when setting SelectedItem for the first time
    /// </summary>
    private bool _updatingSelectedItem;
    public double CurrentWidth { get; set; }


    public void Dispose()
    {
        NavigationManager.Unsubscribe(NavigationHandler);
        RegionCollection.Unsubscribe(RegionCollectionChangedHandler);
    }


    private void NavigationHandler(NavigationContext context)
    {
        if (context.Region != RegionNames.ContentRegion)
            return;

        var itemToUpdate = ItemsSource.FirstOrDefault(x => x.Uri == context.Uri);
        if (itemToUpdate is null) return;
        
        
        _updatingSelectedItem = true;
        SelectedItem = itemToUpdate;
        _updatingSelectedItem = false;
    }

    private void InvokeNavigation()
    {
        ArgumentNullException.ThrowIfNull(SelectedItem);
        NavigationManager.Navigate(RegionNames.ContentRegion, SelectedItem.Uri);
    }

    private void RegionCollectionChangedHandler(RegionCollectionContext context)
    {
        if (context.Region != RegionNames.Sidebar) return;

        if (context.Type is null)
            throw new InvalidOperationException("sidebar items should be registered by providing view model type");

        var attribute = context.Type.GetCustomAttribute<SidebarElementAttribute>();
        if (attribute is null)
            throw new InvalidOperationException(
                $"view models associated with the sidebar must provide {nameof(SidebarElementAttribute)}");

        switch (context.Action)
        {
            case RegionCollectionAction.Add:
                ItemsSource.Add(new SidebarElement
                    { Header = attribute.Header, Icon = attribute.Icon, Uri = attribute.Uri });
                break;
            case RegionCollectionAction.Remove:
                var itemToUpdate = ItemsSource.First(x => x.Uri == attribute.Uri);
                ItemsSource.Remove(itemToUpdate);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(context.Action));
        }

        context.IsHandled = true;
    }
}
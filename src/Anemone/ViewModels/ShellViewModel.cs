using System;
using System.Linq;
using System.Windows.Input;
using Anemone.Core;
using Anemone.Settings;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;
using Microsoft.Xaml.Behaviors.Core;
using Prism.Commands;
using Prism.Regions;

namespace Anemone.ViewModels;

public class ShellViewModel : ViewModelBase
{
    public ShellViewModel(ShellSettings settings, INavigationRegistrations navigationRegistrations,
        ILogger<ShellViewModel> logger, IRegionManager regionManager, IApplicationCommands applicationCommands, ISnackbarMessageQueue snackbarMessageQueue)
    {
        Settings = settings;
        NavigationRegistrations = navigationRegistrations;

        Logger = logger;
        RegionManager = regionManager;
        SnackbarMessageQueue = snackbarMessageQueue;
        SelectedNavigationItemChangedCommand = new ActionCommand(Navigate);
        applicationCommands.NavigateCommand.RegisterCommand(NavigationCommand);
        if (!navigationRegistrations.Any()) return;
        SelectedNavigationItem = navigationRegistrations.First();
    }

    public ICommand SelectedNavigationItemChangedCommand { get; }
    public NavigationPanelItem? SelectedNavigationItem { get; set; }

    public ShellSettings Settings { get; set; }
    public INavigationRegistrations NavigationRegistrations { get; }
    private ILogger<ShellViewModel> Logger { get; }
    private IRegionManager RegionManager { get; }
    public ISnackbarMessageQueue SnackbarMessageQueue { get; set; }

    private DelegateCommand<string>? _navigationCommand;

    private DelegateCommand<string> NavigationCommand => _navigationCommand ??= new DelegateCommand<string>(ExecuteNavigateCommand);

    private void Navigate()
    {
        if (SelectedNavigationItem is null)
            throw new ArgumentNullException(nameof(SelectedNavigationItem));
        RegionManager.RequestNavigate(RegionNames.ContentRegion, SelectedNavigationItem.NavigationPath);
    }

    private void ExecuteNavigateCommand(string navigationPath)
    {
        // update selected item in navigation panel
        var navItem = NavigationRegistrations.SingleOrDefault(x => x.NavigationPath == navigationPath);
        if (navItem is not null)
            SelectedNavigationItem = navItem;
        
        
        RegionManager.RequestNavigate(RegionNames.ContentRegion, navigationPath);
    }
}
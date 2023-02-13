﻿using System;
using System.Linq;
using System.Windows.Input;
using Anemone.Core;
using Anemone.Settings;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;
using Microsoft.Xaml.Behaviors.Core;
using Prism.Regions;

namespace Anemone.ViewModels;

public class ShellViewModel
{
    public ShellViewModel(ShellSettings settings, INavigationRegistrations navigationRegistrations,
        ILogger<ShellViewModel> logger, IRegionManager regionManager, ISnackbarMessageQueue snackbarMessageQueue, IToastService toastService)
    {
        Settings = settings;
        NavigationRegistrations = navigationRegistrations;

        Logger = logger;
        RegionManager = regionManager;
        SnackbarMessageQueue = snackbarMessageQueue;
        SelectedNavigationItemChangedCommand = new ActionCommand(Navigate);
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

    private void Navigate()
    {
        if (SelectedNavigationItem is null)
            throw new ArgumentNullException(nameof(SelectedNavigationItem));
        RegionManager.RequestNavigate(RegionNames.ContentRegion, SelectedNavigationItem.NavigationPath);
    }
}
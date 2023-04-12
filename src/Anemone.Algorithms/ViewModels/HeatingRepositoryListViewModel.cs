using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Anemone.Algorithms.Models;
using Anemone.Core;
using Anemone.Core.Dialogs;
using Anemone.Repository;
using Anemone.Repository.HeatingSystemData;
using Microsoft.Extensions.Logging;
using Prism.Events;
using Prism.Services.Dialogs;
using IDialogService = Anemone.Core.Dialogs.IDialogService;

namespace Anemone.Algorithms.ViewModels;

public class HeatingRepositoryListViewModel : ViewModelBase
{
    private IEnumerable<HeatingSystemNameDisplayModel> _filteredItems = null!;
    private bool _isAscendingOrder = true;
    private List<HeatingSystemNameDisplayModel> _itemsSource = null!;
    private string? _searchString;
    private HeatingSystemNameDisplayModel? _selectedItem;

    public HeatingRepositoryListViewModel(ILogger<HeatingRepositoryListViewModel> logger,
        IHeatingSystemRepository repository, IToastService toastService, IDialogService dialogService,
        IEventAggregator eventAggregator)
    {
        Logger = logger;
        Repository = repository;
        ToastService = toastService;
        DialogService = dialogService;
        EventAggregator = eventAggregator;
        FetchDataCommand = new ActionCommandAsync(ExecuteFetchDataCommand);
        RenameCommand = new ActionCommandAsync(ExecuteRenameCommand);
        DeleteCommand = new ActionCommandAsync(ExecuteDeleteCommand);
        FetchDataCommand.Execute(null);
        FilteredItems = _itemsSource;
    }

    public IEnumerable<HeatingSystemNameDisplayModel> FilteredItems
    {
        get => _filteredItems;
        private set => SetProperty(ref _filteredItems, value);
    }

    public HeatingSystemNameDisplayModel? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value))
                PublishCollectionChangedEvent(value);
        }
    }

    public string? SearchString
    {
        get => _searchString;
        set
        {
            if (SetProperty(ref _searchString, value))
                FilterItems();
        }
    }

    public bool IsAscendingOrder
    {
        get => _isAscendingOrder;
        set
        {
            SetProperty(ref _isAscendingOrder, value);
            OrderItems();
        }
    }

    public bool IsRepositoryListVisible => FilteredItems.Any();


    public ICommand FetchDataCommand { get; }
    public ICommand RenameCommand { get; }
    public ICommand DeleteCommand { get; }


    private ILogger<HeatingRepositoryListViewModel> Logger { get; }
    private IHeatingSystemRepository Repository { get; }
    private IToastService ToastService { get; }
    private IDialogService DialogService { get; }
    private IEventAggregator EventAggregator { get; }


    private void PublishCollectionChangedEvent(HeatingSystemNameDisplayModel? value)
    {
        EventAggregator.GetEvent<HeatingSystemSelectionChangedEvent>().Publish(value);
    }


    private void FilterItems()
    {
        FilteredItems = _itemsSource.Where(x =>
            x.Name.StartsWith(SearchString?.Trim() ?? string.Empty, StringComparison.OrdinalIgnoreCase));
        RaisePropertyChanged(nameof(IsRepositoryListVisible));
    }

    private void OrderItems()
    {
        FilteredItems = IsAscendingOrder
            ? FilteredItems.OrderBy(x => x.Name)
            : FilteredItems.OrderByDescending(x => x.Name);
    }


    private async Task ExecuteFetchDataCommand()
    {
        try
        {
            _itemsSource = (await Repository.GetAllNames())
                .Select(x => new HeatingSystemNameDisplayModel { Id = (int)x.Id!, Name = x.Name })
                .ToList();
            OrderItems();
            RaisePropertyChanged(nameof(IsRepositoryListVisible));
        }
        catch (RepositoryException e)
        {
            DisplayErrorMessage(e);
        }
    }

    private async Task ExecuteRenameCommand()
    {
        HeatingSystem hs;
        try
        {
            hs = await GetRepositoryItem();
        }
        catch (RepositoryException e)
        {
            DisplayErrorMessage(e);
            return;
        }


        var dialogResult = DialogService.ShowTextBoxDialog(hs.Name);
        if (dialogResult.Result != ButtonResult.OK)
            return;


        var newName = dialogResult.Text.Trim();
        if (newName == hs.Name)
            return;

        try
        {
            await UpdateSelectedItemName(newName, hs);
        }
        catch (RepositoryException e)
        {
            DisplayErrorMessage(e);
        }
    }

    private async Task ExecuteDeleteCommand()
    {
        HeatingSystem hs;
        try
        {
            hs = await GetRepositoryItem();
        }
        catch (RepositoryException e)
        {
            DisplayErrorMessage(e);
            return;
        }


        var dialogResult = ShowDeletionDialog(hs.Name);
        if (dialogResult.Result != ButtonResult.OK)
            return;


        try
        {
            await DeleteSelectedItem(hs);
        }
        catch (RepositoryException e)
        {
            DisplayErrorMessage(e);
        }
    }


    private async Task<HeatingSystem> GetRepositoryItem()
    {
        ArgumentNullException.ThrowIfNull(SelectedItem);

        var output = await Repository.Get(SelectedItem.Id);
        return output ?? throw new ArgumentNullException(nameof(output));
    }

    private async Task UpdateSelectedItemName(string newName, HeatingSystem heatingSystem)
    {
        ArgumentNullException.ThrowIfNull(SelectedItem);

        heatingSystem.Name = newName;
        await Repository.Update(heatingSystem);
        SelectedItem.Name = newName;
        RaisePropertyChanged(nameof(FilteredItems));
    }

    private async Task DeleteSelectedItem(HeatingSystem data)
    {
        ArgumentNullException.ThrowIfNull(SelectedItem);

        await Repository.Delete(data);
        _itemsSource.Remove(SelectedItem);
        SelectedItem = null;
        RaisePropertyChanged(nameof(FilteredItems));
        RaisePropertyChanged(nameof(IsRepositoryListVisible));
    }


    private ConfirmationDialogResult ShowDeletionDialog(string heatingSystemName)
    {
        return DialogService.ShowConfirmationDialog($"Are you sure you want to delete {heatingSystemName}",
            cancelButtonText: "Cancel", confirmButtonText: "Delete");
    }


    private void DisplayErrorMessage(RepositoryException e)
    {
        Logger.LogError(e, "There has been a problem while trying to access repository data");
        ToastService.Show("There has been a problem when trying to execute the action.");
    }
}
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Anemone.Algorithms.Models;
using Anemone.Core;
using Anemone.Repository.HeatingSystemData;
using Microsoft.Extensions.Logging;
using Prism.Events;
using Prism.Services.Dialogs;
using IDialogService = Anemone.Core.IDialogService;

namespace Anemone.Algorithms.ViewModels;

public class HeatingRepositoryListViewModel : ViewModelBase
{
    private HeatingSystemListName? _selectedItem;

    public HeatingRepositoryListViewModel(ILogger<HeatingRepositoryListViewModel> logger,
        IHeatingSystemRepository repository, IToastService toastService, IDialogService dialogService, IEventAggregator eventAggregator)
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
        UpdateDataSource = ExecuteFetchDataCommand;
    }


    private ILogger<HeatingRepositoryListViewModel> Logger { get; }
    private IHeatingSystemRepository Repository { get; }
    private IToastService ToastService { get; }
    private IDialogService DialogService { get; }
    private IEventAggregator EventAggregator { get; }
    public ICommand FetchDataCommand { get; set; }
    public ICommand RenameCommand { get; set; }
    public ICommand DeleteCommand { get; set; }
    public Func<Task> UpdateDataSource { get; }

    /// <summary>
    ///     Item on which <see cref="RenameCommand" /> and <see cref="DeleteCommand" /> will be performed.
    /// </summary>
    public HeatingSystemListName? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value))
                PublishEvent(value);
        }
    }

    private void PublishEvent(HeatingSystemListName? value)
    {
        EventAggregator.GetEvent<HeatingSystemSelectionChangedEvent>().Publish(value);
    }

    public ObservableCollection<HeatingSystemListName> ItemsSource { get; set; } = new();

    public async Task<HeatingSystem?> Get(HeatingSystemListName item)
    {
        var result = await Repository.Get(item.Id);
        return result;
    }

    private async Task ExecuteFetchDataCommand()
    {
        var results = await Repository.GetAllNames();
        ItemsSource.Clear();
        foreach (var item in results) ItemsSource.Add(new HeatingSystemListName(item));
    }

    private async Task ExecuteRenameCommand()
    {
        ArgumentNullException.ThrowIfNull(SelectedItem);


        var data = await Repository.Get(SelectedItem.Id!);
        if (data is null)
        {
            OnItemNotFoundInRepository(SelectedItem);
            return;
        }


        var result = DialogService.ShowTextBoxDialog(data.Name);
        if (result.Result != ButtonResult.OK)
            return;

        var newName = result.Text.Trim();
        if (result.Text != data.Name)
        {
            data.Name = newName;
            SelectedItem.Name = newName;
            await Repository.Update(data);
        }
    }

    private async Task ExecuteDeleteCommand()
    {
        ArgumentNullException.ThrowIfNull(SelectedItem);


        var data = await Repository.Get(SelectedItem.Id);
        if (data is null)
        {
            OnItemNotFoundInRepository(SelectedItem);
            return;
        }

        var result = DialogService.ShowConfirmationDialog($"Are you sure you want to delete {data.Name}",
            cancelButtonText: "Cancel", confirmButtonText: "Delete");
        if (result.Result != ButtonResult.OK)
            return;

        await Repository.Delete(data);
        ItemsSource.Remove(SelectedItem);
        SelectedItem = null;
    }

    private void OnItemNotFoundInRepository(HeatingSystemListName item)
    {
        const string errorMessage = "item not found in repository";
        Logger.LogWarning("item {Item} with Id {Id} was not found in the repository", item.Name, item.Id);
        ToastService.Show(errorMessage, "refresh data", () => FetchDataCommand.Execute(null));
        ItemsSource.Remove(item);
    }
}
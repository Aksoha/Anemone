using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Anemone.Core.Common.Entities;
using Anemone.Core.Persistence;
using Anemone.Core.Persistence.HeatingSystem;
using Anemone.Mocks.HeatingSystemData;
using Anemone.UI.Calculation.Models;
using Anemone.UI.Calculation.ViewModels;
using Anemone.UI.Core.Dialogs;
using Anemone.UI.Core.Navigation;
using Anemone.UI.Core.Navigation.Regions;
using Anemone.UI.Core.Notifications;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Events;
using Prism.Services.Dialogs;
using IDialogService = Anemone.UI.Core.Dialogs.IDialogService;

namespace Anemone.UI.Calculation.Tests.ViewModels;

[SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
public class HeatingRepositoryListViewModelTests
{
    private readonly Mock<IDialogService> _dialogMock = new();
    private readonly Mock<IEventAggregator> _eaMock = new();
    private readonly Mock<ILogger<HeatingRepositoryListViewModel>> _loggerMock = new();
    private readonly HeatingSystemRepositoryMock _repositoryMock = new();
    private readonly Mock<IToastService> _toastMock = new();
    private readonly Mock<INavigationManager> _navigationMock = new();

    public HeatingRepositoryListViewModelTests()
    {
        SetInitialMockSetup();
    }

    private ILogger<HeatingRepositoryListViewModel> Logger => _loggerMock.Object;
    private IHeatingSystemRepository Repository => _repositoryMock.Object;
    private IToastService ToastService => _toastMock.Object;
    private IDialogService DialogService => _dialogMock.Object;
    private IEventAggregator EventAggregator => _eaMock.Object;
    private INavigationManager NavigationManager => _navigationMock.Object;


    [Fact]
    public void SelectionChanged_PublishesHeatingSystemSelectionChangedEvent()
    {
        // arrange
        var vm = CreateViewModel(false);
        var model = new HeatingSystemNameDisplayModel();

        // act
        vm.SelectedItem = model;

        // assert
        _eaMock.Verify(x => x.GetEvent<HeatingSystemSelectionChangedEvent>().Publish(model), Times.Once);
    }


    [Fact]
    public void FilteredItems_IsUsingRepositoryEntities()
    {
        // arrange
        _repositoryMock.DeleteAll();
        var repositoryObjects = _repositoryMock.CreateObjectInRepository(5);

        // act
        var vm = CreateViewModel(false);

        // assert
        VerifyRepositoryGetAllNamesCalled(Times.Once);
        Assert.Equal(repositoryObjects.Length, vm.FilteredItems.Count());
        Assert.All(repositoryObjects,
            repositoryObject => { Assert.Contains(vm.FilteredItems, x => x.Id == repositoryObject.Id); });
    }

    [Fact]
    public void SearchString_FiltersFilteredItems()
    {
        // arrange
        _repositoryMock.DeleteAll();
        _repositoryMock.Setup(m => m.GetAllNames()).Returns(() =>
        {
            var list = new List<HeatingSystemName>
            {
                new() { Id = 1, Name = "S235 d=30mm" },
                new() { Id = 2, Name = "S235 d=50mm" },
                new() { Id = 3, Name = "Steel 275" }
            };
            return Task.FromResult(list.AsEnumerable());
        });

        var vm = CreateViewModel(false);


        // act
        vm.SearchString = "S235";


        // arrange
        Assert.Contains(vm.FilteredItems, item => item.Name == "S235 d=30mm");
        Assert.Contains(vm.FilteredItems, item => item.Name == "S235 d=50mm");
        Assert.DoesNotContain(vm.FilteredItems, item => item.Name == "Steel 275");
    }

    [Fact]
    public void SearchString_RisesPropertyChangedOnFilteredItems()
    {
        // arrange
        var vm = CreateViewModel(false);

        // act
        void Act()
        {
            vm.SearchString = "not relevant";
        }

        // assert
        Assert.PropertyChanged(vm, nameof(vm.FilteredItems), Act);
    }


    [Fact]
    public void SearchString_RisesPropertyChangedOnIsRepositoryListVisible()
    {
        // arrange
        var vm = CreateViewModel(false);

        // act
        void Act()
        {
            vm.SearchString = "not relevant";
        }

        // assert
        Assert.PropertyChanged(vm, nameof(vm.IsRepositoryListVisible), Act);
    }


    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsAscendingOrder_OrdersFilteredData(bool orderIsAscending)
    {
        // arrange
        var vm = CreateViewModel(false);

        var orderedCollection = vm.FilteredItems.OrderBy(p => p.Name).ToArray();
        if (orderIsAscending is false)
            orderedCollection = orderedCollection.Reverse().ToArray();


        // act
        vm.IsAscendingOrder = orderIsAscending;


        // assert
        for (var i = 0; i < orderedCollection.Length; i++)
        {
            var expected = orderedCollection[i];
            var actual = vm.FilteredItems.ElementAt(i);
            Assert.Same(expected, actual);
        }
    }


    [Fact]
    public void IsRepositoryListVisible_IsVisibleWhenFilterMatchesAtLeastOneElement()
    {
        // arrange
        _repositoryMock.DeleteAll();
        _repositoryMock.Setup(m => m.GetAllNames()).Returns(() =>
        {
            var list = new List<HeatingSystemName>
            {
                new() { Id = 1, Name = "S235 d=30mm" },
                new() { Id = 2, Name = "S235 d=50mm" },
                new() { Id = 3, Name = "Steel 275" }
            };
            return Task.FromResult(list.AsEnumerable());
        });

        var vm = CreateViewModel(false);

        // act
        vm.SearchString = "S235";


        // assert
        Assert.True(vm.IsRepositoryListVisible);
    }

    [Fact]
    public void IsRepositoryListVisible_IsVisibleWhenFilterMatchesZeroElements()
    {
        // arrange
        _repositoryMock.DeleteAll();
        _repositoryMock.Setup(m => m.GetAllNames()).Returns(() =>
        {
            var list = new List<HeatingSystemName>
            {
                new() { Id = 1, Name = "S235 d=30mm" },
                new() { Id = 2, Name = "S235 d=50mm" },
                new() { Id = 3, Name = "Steel 275" }
            };
            return Task.FromResult(list.AsEnumerable());
        });
        var vm = CreateViewModel(false);


        // act
        vm.SearchString = "Iron";


        // assert
        Assert.False(vm.IsRepositoryListVisible);
    }


    [Fact]
    public void FetchDataCommand_RisesPropertyChangedOnFilteredItems()
    {
        // arrange
        var vm = CreateViewModel(false);

        // act
        void Act()
        {
            vm.FetchDataCommand.Execute(null);
        }

        // assert
        Assert.PropertyChanged(vm, nameof(vm.FilteredItems), Act);
    }

    [Fact]
    public void FetchDataCommand_RisesPropertyChangedOnIsRepositoryListVisible()
    {
        // arrange
        var vm = CreateViewModel(false);

        // act
        void Act()
        {
            vm.FetchDataCommand.Execute(null);
        }

        // assert
        Assert.PropertyChanged(vm, nameof(vm.IsRepositoryListVisible), Act);
    }

    [Fact]
    public void FetchDataCommand_NotifiesUserOnRepositoryException()
    {
        // arrange
        SetupConfirmationDialog(ButtonResult.OK);
        SetupRepositoryException(m => m.GetAllNames());

        // act
        CreateViewModel(false);


        // assert
        VerifyToastShowed(It.IsAny<string>, Times.Once);
    }


    [Fact]
    public void RenameCommand_UpdatesRepositoryWhenDialogIsConfirmed()
    {
        // arrange
        const string newNameOfHeatingSystem = "New name of heating system";
        SetupTextBoxDialog(ButtonResult.OK, newNameOfHeatingSystem);

        var vm = CreateViewModel(false);
        var selectedItem = SetSelectedItem(vm);
        var oldName = selectedItem.Name;
        var oldId = selectedItem.Id;

        // act
        vm.RenameCommand.Execute(null);

        // assert
        VerifyTextBoxDialogShowed(() => oldName, Times.Once);
        VerifyRepositoryGetCalled(() => oldId!, Times.Once);
        VerifyRepositoryUpdateCalled(() => It.Is<HeatingSystem>(p => p.Id == oldId), Times.Once);
        Assert.Equal(newNameOfHeatingSystem, selectedItem.Name);
        Assert.Same(selectedItem, vm.SelectedItem);
    }

    [Fact]
    public void RenameCommand_NotifiesUserOnGetRepositoryException()
    {
        // arrange
        SetupTextBoxDialog(ButtonResult.OK, "unrelated to the test");
        SetupRepositoryException(m => m.Get(It.IsAny<int>()));
        var vm = CreateViewModel(true);

        // act
        vm.RenameCommand.Execute(null);

        // assert
        VerifyToastShowed(It.IsAny<string>, Times.Once);
    }

    [Fact]
    public void RenameCommand_NotifiesUserOnUpdateRepositoryException()
    {
        // arrange
        SetupTextBoxDialog(ButtonResult.OK, "unrelated to the test");
        SetupRepositoryException(m => m.Update(It.IsAny<HeatingSystem>()));
        var vm = CreateViewModel(true);

        // act
        vm.RenameCommand.Execute(null);

        // assert
        VerifyToastShowed(It.IsAny<string>, Times.Once);
    }


    [Theory]
    [InlineData(ButtonResult.Abort)]
    [InlineData(ButtonResult.Cancel)]
    [InlineData(ButtonResult.Ignore)]
    [InlineData(ButtonResult.No)]
    [InlineData(ButtonResult.None)]
    [InlineData(ButtonResult.Retry)]
    public void RenameCommand_DoesNotUpdateRepositoryWhenDialogIsNotConfirmed(ButtonResult buttonResult)
    {
        // arrange
        SetupTextBoxDialog(buttonResult, "the name shouldn't have changed");

        var vm = CreateViewModel(false);
        var selectedItem = SetSelectedItem(vm);
        var oldName = selectedItem.Name;


        // act
        vm.RenameCommand.Execute(null);


        // assert
        VerifyTextBoxDialogShowed(() => selectedItem.Name, Times.Once);
        VerifyRepositoryUpdateCalled(It.IsAny<HeatingSystem>, Times.Never);
        Assert.Equal(oldName, selectedItem.Name);
        Assert.Same(selectedItem, vm.SelectedItem);
    }

    [Fact]
    public void RenameCommand_RisesPropertyChangedOnFilteredItems()
    {
        // arrange
        SetupTextBoxDialog(ButtonResult.OK, "not relevant to this test");
        var vm = CreateViewModel(true);

        // act
        void Act()
        {
            vm.RenameCommand.Execute(null);
        }

        // assert
        Assert.PropertyChanged(vm, nameof(vm.FilteredItems), Act);
    }

    [Fact]
    public void RenameCommand_DoesNotUpdateRepositoryItemWhenHeatingSystemNameIsDidNotChange()
    {
        // arrange
        var vm = CreateViewModel(false);
        var selectedItem = SetSelectedItem(vm);
        SetupTextBoxDialog(ButtonResult.OK, selectedItem.Name);

        // act
        vm.RenameCommand.Execute(null);

        // assert
        VerifyRepositoryUpdateCalled(It.IsAny<HeatingSystem>, Times.Never);
    }


    [Fact]
    public void DeleteCommand_DeletesRepositoryItemWhenDialogIsConfirmed()
    {
        // arrange
        SetupConfirmationDialog(ButtonResult.OK);

        var vm = CreateViewModel(false);
        var itemToDelete = SetSelectedItem(vm);


        // act
        vm.DeleteCommand.Execute(null);


        // assert
        VerifyConfirmationDialogShowed(It.IsAny<string>, Times.Once);
        VerifyRepositoryGetCalled(() => itemToDelete.Id, Times.Once);
        VerifyRepositoryDeleteCalled(() => It.Is<HeatingSystem>(p => p.Id == itemToDelete.Id), Times.Once);
        Assert.Null(vm.SelectedItem);
        Assert.DoesNotContain(vm.FilteredItems, x => x == itemToDelete);
    }

    [Theory]
    [InlineData(ButtonResult.Abort)]
    [InlineData(ButtonResult.Cancel)]
    [InlineData(ButtonResult.Ignore)]
    [InlineData(ButtonResult.No)]
    [InlineData(ButtonResult.None)]
    [InlineData(ButtonResult.Retry)]
    public void DeleteCommand_DoesNotDeleteRepositoryItemWhenDialogIsNotConfirmed(ButtonResult buttonResult)
    {
        // arrange
        SetupConfirmationDialog(buttonResult);

        var vm = CreateViewModel(false);
        var itemToDelete = SetSelectedItem(vm);


        // act
        vm.DeleteCommand.Execute(null);


        // assert
        VerifyConfirmationDialogShowed(It.IsAny<string>, Times.Once);
        VerifyRepositoryDeleteCalled(It.IsAny<HeatingSystem>, Times.Never);
        Assert.Contains(vm.FilteredItems, x => x == itemToDelete);
        Assert.Same(itemToDelete, vm.SelectedItem);
    }

    [Fact]
    public void DeleteCommand_RisesPropertyChangedOnFilteredItems()
    {
        // arrange
        SetupConfirmationDialog(ButtonResult.OK);
        var vm = CreateViewModel(true);

        // act
        void Act()
        {
            vm.DeleteCommand.Execute(null);
        }

        // assert
        Assert.PropertyChanged(vm, nameof(vm.FilteredItems), Act);
    }

    [Fact]
    public void DeleteCommand_RisesPropertyChangedOnIsRepositoryListVisible()
    {
        // arrange
        SetupConfirmationDialog(ButtonResult.OK);
        var vm = CreateViewModel(true);

        // act
        void Act()
        {
            vm.DeleteCommand.Execute(null);
        }

        // assert
        Assert.PropertyChanged(vm, nameof(vm.IsRepositoryListVisible), Act);
    }

    [Fact]
    public void DeleteCommand_NotifiesUserOnGetRepositoryException()
    {
        // arrange
        const string newNameOfHeatingSystem = "New name of heating system";
        SetupTextBoxDialog(ButtonResult.OK, newNameOfHeatingSystem);
        SetupRepositoryException(m => m.Get(It.IsAny<int>()));
        var vm = CreateViewModel(true);

        // act
        vm.DeleteCommand.Execute(null);

        // assert
        VerifyToastShowed(It.IsAny<string>, Times.Once);
    }

    [Fact]
    public void DeleteCommand_NotifiesUserOnDeleteRepositoryException()
    {
        // arrange
        SetupConfirmationDialog(ButtonResult.OK);
        SetupRepositoryException(m => m.Delete(It.IsAny<HeatingSystem>()));
        var vm = CreateViewModel(true);

        // act
        vm.DeleteCommand.Execute(null);

        // assert
        VerifyToastShowed(It.IsAny<string>, Times.Once);
    }

    [Fact]
    public void NavigateToDataImportCommand_Navigates()
    {
        // arrange
        var vm = CreateViewModel(false);
        
        
        // act
        vm.NavigateToDataImportCommand.Execute(null);
        
        
        // assert
        _navigationMock.Verify(m => m.Navigate(RegionNames.ContentRegion, NavigationNames.DataImport), Times.Once);
    }

    private void SetInitialMockSetup()
    {
        _eaMock.Setup(x =>
            x.GetEvent<HeatingSystemSelectionChangedEvent>().Publish(It.IsAny<HeatingSystemNameDisplayModel>()));
        _repositoryMock.CreateObjectInRepository(2);
    }

    private HeatingRepositoryListViewModel CreateViewModel(bool setSelectedItem)
    {
        var vm = new HeatingRepositoryListViewModel(Logger, Repository, ToastService, DialogService, EventAggregator, NavigationManager);

        if (setSelectedItem)
            SetSelectedItem(vm);

        return vm;
    }

    private static HeatingSystemNameDisplayModel SetSelectedItem(HeatingRepositoryListViewModel vm)
    {
        var selectedItem = vm.FilteredItems.First();
        vm.SelectedItem = selectedItem;
        return selectedItem;
    }

    private void SetupTextBoxDialog(ButtonResult buttonResult, string returnedText)
    {
        _dialogMock.Setup(x => x.ShowTextBoxDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(() =>
        {
            var dialogResult = new TextBoxDialogResult
            {
                Result = buttonResult,
                Text = returnedText
            };
            return dialogResult;
        });
    }

    private void SetupConfirmationDialog(ButtonResult buttonResult)
    {
        _dialogMock.Setup(m =>
                m.ShowConfirmationDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>()))
            .Returns(() =>
            {
                var dialogResult = new ConfirmationDialogResult
                {
                    Result = buttonResult
                };
                return dialogResult;
            });
    }

    private void SetupRepositoryException(Expression<Action<IHeatingSystemRepository>> expression)
    {
        _repositoryMock.Setup(expression).Throws(new RepositoryWriteException("not relevant to the test"));
    }


    private void VerifyTextBoxDialogShowed(Func<string> textDelegate, Func<Times> times)
    {
        _dialogMock.Verify(m => m.ShowTextBoxDialog(textDelegate.Invoke(), It.IsAny<string>()), times);
    }

    private void VerifyConfirmationDialogShowed(Func<string> messageDelegate, Func<Times> times)
    {
        _dialogMock.Verify(
            m => m.ShowConfirmationDialog(messageDelegate.Invoke(), It.IsAny<string>(), "Cancel", "Delete"), times);
    }

    private void VerifyToastShowed(Func<string> message, Func<Times> times)
    {
        _toastMock.Verify(m => m.Show(message.Invoke()), times);
    }


    private void VerifyRepositoryGetCalled(Func<int> idDelegate, Func<Times> times)
    {
        _repositoryMock.Verify(m => m.Get(idDelegate.Invoke()), times);
    }

    private void VerifyRepositoryGetAllNamesCalled(Func<Times> times)
    {
        _repositoryMock.Verify(m => m.GetAllNames(), times);
    }

    private void VerifyRepositoryUpdateCalled(Func<HeatingSystem> heatingSystemDelegate, Func<Times> times)
    {
        _repositoryMock.Verify(m => m.Update(heatingSystemDelegate.Invoke()), times);
    }

    private void VerifyRepositoryDeleteCalled(Func<HeatingSystem> heatingSystemDelegate, Func<Times> times)
    {
        _repositoryMock.Verify(m => m.Delete(heatingSystemDelegate.Invoke()), times);
    }
}
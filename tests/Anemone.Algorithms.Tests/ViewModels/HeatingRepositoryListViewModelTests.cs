using Anemone.Algorithms.Models;
using Anemone.Algorithms.ViewModels;
using Anemone.Core;
using Anemone.RepositoryMock.HeatingSystemData;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Events;
using Prism.Services.Dialogs;
using IDialogService = Anemone.Core.IDialogService;

namespace Anemone.Algorithms.Tests.ViewModels;

public class HeatingRepositoryListViewModelTests
{
    [Fact]
    public async Task Rename_WhenDialogIsConfirmed()
    {
        // arrange
        var logger = Mock.Of<ILogger<HeatingRepositoryListViewModel>>();
        var toastService = Mock.Of<IToastService>();
        var dialogServiceMock = new Mock<IDialogService>();
        var eventAggregatorMock = new Mock<IEventAggregator>();
        eventAggregatorMock.Setup(x => x.GetEvent<HeatingSystemSelectionChangedEvent>().Publish(It.IsAny<HeatingSystemListName>()));
        var eventAggregator = eventAggregatorMock.Object;
        const string newName = "hs new name";

        var testContext = HeatingSystemFaker.GenerateHeatingSystem(20).ToList();
        var repository = new HeatingSystemRepositoryMock();
        foreach (var item in testContext) await repository.Create(item);


        dialogServiceMock.Setup(x => x.ShowTextBoxDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(() =>
        {
            var dialogResult = new TextBoxDialogResult();
            dialogResult.Result = ButtonResult.OK;
            dialogResult.Text = newName;
            return dialogResult;
        });

        var dialogService = dialogServiceMock.Object;
        var testedModel = new HeatingRepositoryListViewModel(logger, repository, toastService, dialogService, eventAggregator);

        // act
        var itemToUpdate = testedModel.ItemsSource.First();
        testedModel.SelectedItem = itemToUpdate;
        testedModel.RenameCommand.Execute(null);

        // assert
        Assert.Equal(newName, itemToUpdate.Name);
        Assert.Same(itemToUpdate, testedModel.SelectedItem);
        Assert.Equal(newName, (await repository.Get(itemToUpdate.Id))!.Name);
    }

    [Theory]
    [InlineData(ButtonResult.Abort)]
    [InlineData(ButtonResult.Cancel)]
    [InlineData(ButtonResult.Ignore)]
    [InlineData(ButtonResult.No)]
    [InlineData(ButtonResult.None)]
    [InlineData(ButtonResult.Retry)]
    public async Task Rename_WhenDialogIsNotConfirmed(ButtonResult buttonResult)
    {
        // arrange
        var logger = Mock.Of<ILogger<HeatingRepositoryListViewModel>>();
        var toastService = Mock.Of<IToastService>();
        var dialogServiceMock = new Mock<IDialogService>();
        var eventAggregatorMock = new Mock<IEventAggregator>();
        eventAggregatorMock.Setup(x => x.GetEvent<HeatingSystemSelectionChangedEvent>().Publish(It.IsAny<HeatingSystemListName>()));
        var eventAggregator = eventAggregatorMock.Object;
        const string newName = "hs new name";

        var testContext = HeatingSystemFaker.GenerateHeatingSystem(20).ToList();
        var repository = new HeatingSystemRepositoryMock();
        foreach (var item in testContext) await repository.Create(item);

        dialogServiceMock.Setup(x => x.ShowTextBoxDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(() =>
        {
            var dialogResult = new TextBoxDialogResult();
            dialogResult.Result = buttonResult;
            dialogResult.Text = newName;
            return dialogResult;
        });

        var dialogService = dialogServiceMock.Object;
        var testedModel = new HeatingRepositoryListViewModel(logger, repository, toastService, dialogService, eventAggregator);

        // act
        var itemToUpdate = testedModel.ItemsSource.First();
        var oldName = itemToUpdate.Name;
        testedModel.SelectedItem = itemToUpdate;
        testedModel.RenameCommand.Execute(null);

        // assert
        Assert.Equal(oldName, itemToUpdate.Name);
        Assert.Same(itemToUpdate, testedModel.SelectedItem);
        Assert.Equal(oldName, (await repository.Get(itemToUpdate.Id))!.Name);
    }


    [Fact]
    public async Task Delete_WhenDialogIsConfirmed()
    {
        // arrange
        var logger = Mock.Of<ILogger<HeatingRepositoryListViewModel>>();
        var toastService = Mock.Of<IToastService>();
        var dialogServiceMock = new Mock<IDialogService>();
        var eventAggregatorMock = new Mock<IEventAggregator>();
        eventAggregatorMock.Setup(x => x.GetEvent<HeatingSystemSelectionChangedEvent>().Publish(It.IsAny<HeatingSystemListName>()));
        var eventAggregator = eventAggregatorMock.Object;
        
        
        dialogServiceMock.Setup(x =>
                x.ShowConfirmationDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>()))
            .Returns(() =>
            {
                var dialogResult = new ConfirmationDialogResult();
                dialogResult.Result = ButtonResult.OK;
                return dialogResult;
            });

        var dialogService = dialogServiceMock.Object;

        var data = HeatingSystemFaker.GenerateHeatingSystem(20).ToList();
        var repository = new HeatingSystemRepositoryMock();
        foreach (var item in data) await repository.Create(item);

        var testedModel = new HeatingRepositoryListViewModel(logger, repository, toastService, dialogService, eventAggregator);


        // act
        var itemToDelete = testedModel.ItemsSource.First();
        testedModel.SelectedItem = itemToDelete;
        testedModel.DeleteCommand.Execute(null);


        // assert
        Assert.Null(await repository.Get(itemToDelete.Id));
        Assert.DoesNotContain(testedModel.ItemsSource, s => s == itemToDelete);
        Assert.Null(testedModel.SelectedItem);
    }

    [Theory]
    [InlineData(ButtonResult.Abort)]
    [InlineData(ButtonResult.Cancel)]
    [InlineData(ButtonResult.Ignore)]
    [InlineData(ButtonResult.No)]
    [InlineData(ButtonResult.None)]
    [InlineData(ButtonResult.Retry)]
    public async Task Delete_WhenDialogIsNotConfirmed(ButtonResult buttonResult)
    {
        // arrange
        var logger = Mock.Of<ILogger<HeatingRepositoryListViewModel>>();
        var toastService = Mock.Of<IToastService>();
        var dialogServiceMock = new Mock<IDialogService>();
        var eventAggregatorMock = new Mock<IEventAggregator>();
        eventAggregatorMock.Setup(x => x.GetEvent<HeatingSystemSelectionChangedEvent>().Publish(It.IsAny<HeatingSystemListName>()));
        var eventAggregator = eventAggregatorMock.Object;


        dialogServiceMock.Setup(x =>
                x.ShowConfirmationDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>()))
            .Returns(() =>
            {
                var dialogResult = new ConfirmationDialogResult();
                dialogResult.Result = buttonResult;
                return dialogResult;
            });

        var dialogService = dialogServiceMock.Object;

        var data = HeatingSystemFaker.GenerateHeatingSystem(20).ToList();
        var repository = new HeatingSystemRepositoryMock();
        foreach (var item in data) await repository.Create(item);

        var testedModel = new HeatingRepositoryListViewModel(logger, repository, toastService, dialogService, eventAggregator);


        // act
        var itemToDelete = testedModel.ItemsSource.First();
        testedModel.SelectedItem = itemToDelete;
        testedModel.DeleteCommand.Execute(null);


        // assert
        Assert.NotNull(await repository.Get(itemToDelete.Id));
        Assert.Contains(testedModel.ItemsSource, s => s == itemToDelete);
        Assert.Same(itemToDelete, testedModel.SelectedItem);
    }
}
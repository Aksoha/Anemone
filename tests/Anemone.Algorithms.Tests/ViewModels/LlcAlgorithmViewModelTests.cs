using Anemone.Algorithms.Builders;
using Anemone.Algorithms.Matching;
using Anemone.Algorithms.Models;
using Anemone.Algorithms.Report;
using Anemone.Algorithms.ViewModels;
using Anemone.Core;
using Anemone.Repository.HeatingSystemData;
using Anemone.RepositoryMock.HeatingSystemData;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Events;

namespace Anemone.Algorithms.Tests.ViewModels;

public class LlcAlgorithmViewModelTests
{
    private readonly HeatingSystemRepositoryMock _repositoryMock = new();
    private readonly Mock<IValidator<LlcMatchingBuildArgs>> _validatorMock = new();
    private readonly Mock<IToastService> _toastMock = new();
    private readonly Mock<ILogger<LlcAlgorithmViewModel>> _loggerMock = new();
    private readonly Mock<IEventAggregator> _eaMock = new();
    private readonly Mock<ILlcMatchingCalculator> _mcMock = new();
    private readonly Mock<IReportGenerator> _rgMock = new();
    private readonly Mock<IDataExporter> _deMock = new();
    private readonly Mock<ISaveFileDialog> _sdMock = new();

    private IHeatingSystemRepository Repository => _repositoryMock.Object;
    private IValidator<LlcMatchingBuildArgs> Validator => _validatorMock.Object;
    private IToastService ToastService => _toastMock.Object;
    private ILogger<LlcAlgorithmViewModel> Logger => _loggerMock.Object;
    private IEventAggregator EventAggregator => _eaMock.Object;
    private ILlcMatchingCalculator MatchingCalculator => _mcMock.Object;
    private IReportGenerator ReportGenerator => _rgMock.Object;
    private IDataExporter DataExporter => _deMock.Object;
    private ISaveFileDialog Dialog => _sdMock.Object;

    public LlcAlgorithmViewModelTests()
    {
        SetInitialMockSetup();
    }
    
    [Fact]
    public void NotifiesUserOnCalculationError()
    {
        // arrange
        _mcMock.Setup(m => m.Calculate(It.IsAny<LlcMatchingParameter>(), It.IsAny<HeatingSystem>()))
            .Throws<Exception>();

        var vm = CreateViewModel();

        // act
        vm.CalculateCommand.Execute(null);
        
        // assert
        _toastMock.Verify(m => m.Show(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void ValidatesDataBeforeCalculation()
    {
        // arrange
        var vm = CreateViewModel();
        PublishSelectionChangedEvent();
        
        // act
        vm.CalculateCommand.Execute(null);

        // assert
        _validatorMock.Verify(m => m.Validate(It.IsAny<LlcMatchingBuildArgs>()), Times.Once);
    }

    private HeatingSystem PublishSelectionChangedEvent()
    {
        var hs = _repositoryMock.CreateObjectInRepository();
        EventAggregator.GetEvent<HeatingSystemSelectionChangedEvent>().Publish(new HeatingSystemNameDisplayModel(hs));
        return hs;
    }

    [Fact]
    public async Task DoesNotStartCalculationOnInvalidData()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task StartsCalculationOnValidData()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task DoesNotStartCalculationWhenHeatingSystemIsNotSelected()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task DoesNotStartCalculationWhenParametersAreNotFilled()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task DoesNotShowResultWhenCalculationIsNotPerformed()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task ShowsResultsWhenCalculationIsPerformed()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task CancelCommandCancelsCalculation()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public async Task NotifiesUserOnInvalidData()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task ExportCommandExportsData()
    {
        throw new NotImplementedException();
    }
    



    private LlcAlgorithmViewModel CreateViewModel()
    {
        return new LlcAlgorithmViewModel(Repository, Validator, ToastService, Logger, EventAggregator,
            MatchingCalculator, ReportGenerator, DataExporter, Dialog);
    }

    private void SetInitialMockSetup()
    {
        _eaMock.Setup(m => m.GetEvent<HeatingSystemSelectionChangedEvent>())
            .Returns(new HeatingSystemSelectionChangedEvent());
    }
}
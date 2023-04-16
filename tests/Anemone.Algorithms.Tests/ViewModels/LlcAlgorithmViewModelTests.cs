using System.Data;
using System.Diagnostics.CodeAnalysis;
using Anemone.Algorithms.Builders;
using Anemone.Algorithms.Matching;
using Anemone.Algorithms.Models;
using Anemone.Algorithms.Report;
using Anemone.Algorithms.ViewModels;
using Anemone.Core;
using Anemone.Core.Dialogs;
using Anemone.Repository.HeatingSystemData;
using Anemone.RepositoryMock.HeatingSystemData;
using FluentValidation;
using FluentValidation.Results;
using MatchingAlgorithm;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Events;
using HeatingSystem = Anemone.Repository.HeatingSystemData.HeatingSystem;

namespace Anemone.Algorithms.Tests.ViewModels;

public class LlcAlgorithmViewModelTests
{
    private readonly Mock<CalculationFinishedEvent> _cfeMock = new();
    private readonly Mock<IDataExporter> _deMock = new();
    private readonly Mock<IEventAggregator> _eaMock = new();
    private readonly Mock<ILogger<LlcAlgorithmViewModel>> _loggerMock = new();
    private readonly Mock<ILlcMatchingCalculator> _mcMock = new();
    private readonly HeatingSystemRepositoryMock _repositoryMock = new();
    private readonly Mock<IReportGenerator> _rgMock = new();
    private readonly Mock<ISaveFileDialog> _sdMock = new();
    private readonly Mock<IToastService> _toastMock = new();
    private readonly Mock<IValidator<LlcMatchingBuildArgs>> _validatorMock = new();

    public LlcAlgorithmViewModelTests()
    {
        SetInitialMockSetup();
    }

    private IHeatingSystemRepository Repository => _repositoryMock.Object;
    private IValidator<LlcMatchingBuildArgs> Validator => _validatorMock.Object;
    private IToastService ToastService => _toastMock.Object;
    private ILogger<LlcAlgorithmViewModel> Logger => _loggerMock.Object;
    private IEventAggregator EventAggregator => _eaMock.Object;
    private ILlcMatchingCalculator MatchingCalculator => _mcMock.Object;
    private IReportGenerator ReportGenerator => _rgMock.Object;
    private IDataExporter DataExporter => _deMock.Object;
    private ISaveFileDialog Dialog => _sdMock.Object;
    private CalculationFinishedEvent CalculationFinishedEvent => _cfeMock.Object;


    private LlcAlgorithmViewModel? ViewModel { get; set; }
    private HeatingSystem? HeatingSystem { get; set; }


    [Fact]
    public async Task CalculateCommand_PerformsCalculationWhenDataIsValid()
    {
        // arrange
        CreateViewModelWithValidSetup();

        var matchingResult = new LlcMatchingResultSummary();
        SetupMatchingCalculator(It.IsAny<LlcMatchingParameters>, () => HeatingSystem, matchingResult);


        // act
        await StartCalculation(ViewModel);


        // assert
        VerifyRepositoryGetCalled(() => (int)HeatingSystem.Id!, Times.Once);
        // VerifyValidatorCalled(() => It.Is<LlcMatchingBuildArgs>(p => p.HeatingSystem == HeatingSystem), Times.Once);

        _validatorMock.Verify(m =>
                m.ValidateAsync(It.IsAny<ValidationContext<LlcMatchingBuildArgs>>(), It.IsAny<CancellationToken>()),
            Times.Once);


        VerifyMatchingCalculatorCalled(It.IsAny<LlcMatchingParameters>, () => HeatingSystem, Times.Once);
        VerifyChartsUpdated(It.IsAny<MatchingResultPoint[]>, Times.Once);
        Assert.Same(matchingResult, ViewModel.MatchingResult);
        Assert.True(ViewModel.IsResultCalculated);
        Assert.True(ViewModel.CanExecuteExportDataCommand);
    }

    [Fact]
    public async Task CalculateCommand_DoesNotStartCalculationWhenHeatingSystemIdIsNull()
    {
        // arrange
        SetupValidator(It.IsAny<LlcMatchingBuildArgs>, new ValidationResult());
        SetupMatchingCalculator(It.IsAny<LlcMatchingParameters>, It.IsAny<HeatingSystem>,
            new LlcMatchingResultSummary());

        CreateViewModel();


        // act
        await StartCalculation(ViewModel);


        // assert
        VerifyToastShowed(It.IsAny<string>, Times.Once);
        VerifyValidatorCalled(It.IsAny<LlcMatchingBuildArgs>, Times.Never);
        VerifyMatchingCalculatorCalled(It.IsAny<LlcMatchingParameters>, It.IsAny<HeatingSystem>, Times.Never);
    }

    [Fact]
    public async Task CalculateCommand_DoesNotStartCalculationWhenInputParametersAreNotValid()
    {
        // arrange
        CreateViewModelWithValidSetup();
        SetupValidationFailure();


        // act
        await StartCalculation(ViewModel);


        // assert
        VerifyToastShowed(It.IsAny<string>, Times.Once);
        _validatorMock.Verify(m =>
                m.ValidateAsync(It.IsAny<ValidationContext<LlcMatchingBuildArgs>>(), It.IsAny<CancellationToken>()),
            Times.Once);
        VerifyMatchingCalculatorCalled(It.IsAny<LlcMatchingParameters>, It.IsAny<HeatingSystem>, Times.Never);
    }

    [Fact]
    public async Task CalculateCommand_DisplaysSolutionNotFoundErrors()
    {
        // arrange
        CreateViewModelWithValidSetup();
        SetupMatchingCalculatorException<SolutionNotFoundException>();


        // act
        await StartCalculation(ViewModel);


        // assert
        VerifyToastShowed(It.IsAny<string>, Times.Once);
    }

    [Fact]
    public async Task CalculateCommand_SetsCalculationInProgress()
    {
        // arrange
        CreateViewModelWithValidSetup();
        SetupCalculationDelay();


        // assert
        Assert.False(ViewModel.CalculationInProgress);
        var task = StartCalculation(ViewModel);
        Assert.True(ViewModel.CalculationInProgress);
        await task;
        Assert.False(ViewModel.CalculationInProgress);
    }

    [Fact]
    public async Task CalculateCommand_RisesPropertyChangedOnCanExecuteExportDataCommand()
    {
        // arrange
        CreateViewModelWithValidSetup();


        // act
        Task Act()
        {
            return StartCalculation(ViewModel);
        }


        // assert
        await Assert.PropertyChangedAsync(ViewModel, nameof(ViewModel.CanExecuteExportDataCommand), Act);
        Assert.True(ViewModel.CanExecuteExportDataCommand);
    }

    [Fact]
    public async Task CancellationToken_CancelsCalculation()
    {
        // arrange
        CreateViewModelWithValidSetup();
        _validatorMock.Setup(m =>
                m.ValidateAsync(It.IsAny<ValidationContext<LlcMatchingBuildArgs>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException(), TimeSpan.FromMilliseconds(200));


        // act
        var task = StartCalculation(ViewModel);
        ViewModel.CancelCalculateCommand.Execute(null);


        // assert
        await task;
        Assert.False(ViewModel.CalculationInProgress);
    }

    [Fact]
    public void CalculationInProgress_RisesPropertyChanged()
    {
        // arrange
        CreateViewModelWithValidSetup();


        // act
        void Act()
        {
            ViewModel.CalculationInProgress = !ViewModel.CalculationInProgress;
        }


        // assert
        Assert.PropertyChanged(ViewModel, nameof(ViewModel.CalculationInProgress), Act);
    }

    [Fact]
    public void IsResultCalculated_RisesPropertyChanged()
    {
        // arrange
        CreateViewModelWithValidSetup();


        // act
        void Act()
        {
            ViewModel.IsResultCalculated = !ViewModel.IsResultCalculated;
        }


        // assert
        Assert.PropertyChanged(ViewModel, nameof(ViewModel.IsResultCalculated), Act);
    }

    [Fact]
    public void ExportCommandExportsData_ExportsWhenDataIsValid()
    {
        // arrange
        var dt = new DataTable();
        const string fileName = "filename";
        _sdMock.Setup(m => m.ShowDialog()).Returns(true);
        _sdMock.Setup(p => p.FileName).Returns(fileName);
        _rgMock.Setup(m => m.CreateSheetReport(It.IsAny<MatchingResultSummaryBase>())).Returns(dt);

        CreateViewModel();
        var matchingResults = new LlcMatchingResultSummary();
        ViewModel.MatchingResult = matchingResults;


        // act
        ViewModel.ExportDataCommand.Execute(null);


        // arrange
        VerifyDataExporterCalled(() => fileName, () => dt, Times.Once);
        VerifyReportGeneratorCalled(() => matchingResults, Times.Once);
        VerifyToastShowed(It.IsAny<string>, Times.Once);
        VerifySaveDialogReset();
    }


    [Fact]
    public void ExportDataCommand_DoesNotExportWhenDialogIsCancelled()
    {
        // arrange
        CreateViewModelWithValidSetup();

        _sdMock.Setup(m => m.ShowDialog()).Returns(false);
        var matchingResults = new LlcMatchingResultSummary();
        ViewModel.MatchingResult = matchingResults;


        // act
        ViewModel.ExportDataCommand.Execute(null);


        // assert
        VerifyDataExporterCalled(It.IsAny<string>, It.IsAny<DataTable>, Times.Never);
        VerifyReportGeneratorCalled(It.IsAny<LlcMatchingResultSummary>, Times.Never);
    }


    private void SetInitialMockSetup()
    {
        var selectionChangedEvent = new HeatingSystemSelectionChangedEvent();
        _eaMock.Setup(m => m.GetEvent<HeatingSystemSelectionChangedEvent>()).Returns(selectionChangedEvent);
        _eaMock.Setup(m => m.GetEvent<CalculationFinishedEvent>()).Returns(CalculationFinishedEvent);
    }

    [MemberNotNull(nameof(ViewModel))]
    private void CreateViewModel()
    {
        ViewModel = new LlcAlgorithmViewModel(Repository, Validator, ToastService, Logger, EventAggregator,
            MatchingCalculator, ReportGenerator, DataExporter, Dialog);
    }


    [MemberNotNull(nameof(HeatingSystem))]
    [MemberNotNull(nameof(ViewModel))]
    private void CreateViewModelWithValidSetup()
    {
        HeatingSystem = _repositoryMock.CreateObjectInRepository();

        SetupValidator(It.IsAny<LlcMatchingBuildArgs>, new ValidationResult());
        SetupMatchingCalculator(It.IsAny<LlcMatchingParameters>, It.IsAny<HeatingSystem>,
            new LlcMatchingResultSummary());

        CreateViewModel();
        PublishSelectionChangedEvent(HeatingSystem);
    }

    private void PublishSelectionChangedEvent(HeatingSystem heatingSystem)
    {
        EventAggregator.GetEvent<HeatingSystemSelectionChangedEvent>()
            .Publish(new HeatingSystemNameDisplayModel(heatingSystem));
    }


    private void SetupValidator(Func<LlcMatchingBuildArgs> func, ValidationResult validationResult)
    {
        _validatorMock.Setup(m => m.ValidateAsync(func.Invoke(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
    }

    private void SetupValidationFailure()
    {
        _validatorMock.Setup(m =>
                m.ValidateAsync(It.IsAny<ValidationContext<LlcMatchingBuildArgs>>(), It.IsAny<CancellationToken>()))
            .Throws(new ValidationException("validation has failed"));
    }

    private void SetupValidationDelay()
    {
        _validatorMock.Setup(m => m.ValidateAsync(It.IsAny<LlcMatchingBuildArgs>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(), TimeSpan.FromMilliseconds(200));
    }

    private void SetupCalculationDelay()
    {
        SetupValidationDelay();
    }

    private void SetupMatchingCalculator(Func<LlcMatchingParameters> parameters, Func<HeatingSystem> hs,
        LlcMatchingResultSummary matchingResultSummary)
    {
        _mcMock.Setup(m => m.Calculate(parameters.Invoke(), hs.Invoke()))
            .Returns(Task.FromResult(matchingResultSummary));
    }


    private void SetupMatchingCalculatorException<TException>() where TException : Exception, new()
    {
        _mcMock.Setup(m => m.Calculate(It.IsAny<LlcMatchingParameters>(), It.IsAny<HeatingSystem>()))
            .Throws<TException>();
    }


    private void VerifyRepositoryGetCalled(Func<int> idFunc, Func<Times> times)
    {
        _repositoryMock.Verify(m => m.Get(idFunc.Invoke()), times);
    }

    private void VerifyValidatorCalled(Func<LlcMatchingBuildArgs> buildArgsFunc, Func<Times> times)
    {
        _validatorMock.Verify(m => m.ValidateAsync(buildArgsFunc.Invoke(), It.IsAny<CancellationToken>()), times);
    }

    private void VerifyMatchingCalculatorCalled(Func<LlcMatchingParameters> parametersFunc, Func<HeatingSystem> hsFunc,
        Func<Times> times)
    {
        _mcMock.Verify(m => m.Calculate(parametersFunc.Invoke(), hsFunc.Invoke()), times);
    }

    private void VerifyChartsUpdated(Func<MatchingResultPoint[]> func, Func<Times> times)
    {
        _cfeMock.Verify(m => m.Publish(func.Invoke()), times);
    }


    private void VerifyReportGeneratorCalled(Func<LlcMatchingResultSummary> result, Func<Times> times)
    {
        _rgMock.Verify(m => m.CreateSheetReport(result.Invoke()), times);
    }

    private void VerifyDataExporterCalled(Func<string> fileName, Func<DataTable> data, Func<Times> times)
    {
        _deMock.Verify(m => m.ExportToCsv(fileName.Invoke(), data.Invoke()), times);
    }

    private void VerifySaveDialogReset()
    {
        _sdMock.Verify(m => m.Reset(), Times.Once);
    }


    private void VerifyToastShowed(Func<string> message, Func<Times> times)
    {
        _toastMock.Verify(m => m.Show(message.Invoke()), times);
    }


    private static Task StartCalculation(LlcAlgorithmViewModel viewModel)
    {
        return viewModel.CalculateCommand.ExecuteAsync(null);
    }
}
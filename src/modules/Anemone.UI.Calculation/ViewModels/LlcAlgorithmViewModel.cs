using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Anemone.Core.Common.ValueObjects;
using Anemone.Core.EnergyMatching;
using Anemone.Core.EnergyMatching.Builders;
using Anemone.Core.EnergyMatching.Parameters;
using Anemone.Core.EnergyMatching.Results;
using Anemone.Core.Export;
using Anemone.Core.Persistence.HeatingSystem;
using Anemone.Core.ReportGenerator;
using Anemone.UI.Calculation.Models;
using Anemone.UI.Calculation.Views;
using Anemone.UI.Core;
using Anemone.UI.Core.Commands;
using Anemone.UI.Core.Dialogs;
using Anemone.UI.Core.Icons;
using Anemone.UI.Core.Navigation;
using Anemone.UI.Core.Notifications;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Prism.Events;
using HeatingSystem = Anemone.Core.Common.Entities.HeatingSystem;

namespace Anemone.UI.Calculation.ViewModels;

[SidebarElement("LLC", PackIconKind.HeatingSystemMatching, NavigationNames.Calculation)]
public class LlcAlgorithmViewModel : ViewModelBase
{
    private bool _calculationInProgress;
    private CancellationTokenSource? _cts;
    private bool _isResultCalculated;
    private LlcMatchingResultSummary? _matchingResult;


    public LlcAlgorithmViewModel(IHeatingSystemRepository repository,
        IValidator<LlcMatchingBuildArgs> validator, IToastService toastService,
        ILogger<LlcAlgorithmViewModel> logger,
        IEventAggregator eventAggregator, ILlcMatchingCalculator matchingCalculator,
        IReportGenerator reportGenerator, IDataExporter dataExporter, ISaveFileDialog saveFileDialog)
    {
        Repository = repository;
        Validator = validator;
        ToastService = toastService;
        Logger = logger;
        EventAggregator = eventAggregator;
        MatchingCalculator = matchingCalculator;
        ReportGenerator = reportGenerator;
        DataExporter = dataExporter;
        SaveFileDialog = saveFileDialog;

        CalculateCommand = new DelegateCommandAsync(TryExecuteCalculateCommand);
        CancelCalculateCommand = new ActionCommandAsync(ExecuteCancelCalculateCommand);
        ExportDataCommand =
            new DelegateCommandAsync(ExecuteExportDataCommand).ObservesCanExecute(() => CanExecuteExportDataCommand);
        EventAggregator.GetEvent<HeatingSystemSelectionChangedEvent>().Subscribe(HeatingSystemSelectionChangedHandler);
    }

    public LlcMatchingParameters InputParameters { get; } = new();

    public LlcMatchingResultSummary? MatchingResult
    {
        get => _matchingResult;
        set
        {
            if (SetProperty(ref _matchingResult, value))
                RaisePropertyChanged(nameof(CanExecuteExportDataCommand));
        }
    }

    public bool IsResultCalculated
    {
        get => _isResultCalculated;
        set => SetProperty(ref _isResultCalculated, value);
    }

    public bool CalculationInProgress
    {
        get => _calculationInProgress;
        set => SetProperty(ref _calculationInProgress, value);
    }

    public ICommandAsync CalculateCommand { get; }
    public ICommandAsync CancelCalculateCommand { get; }
    public ICommandAsync ExportDataCommand { get; }
    public bool CanExecuteExportDataCommand => MatchingResult is not null;


    private ILogger<LlcAlgorithmViewModel> Logger { get; }
    private IToastService ToastService { get; }
    private IEventAggregator EventAggregator { get; }
    private ISaveFileDialog SaveFileDialog { get; }
    private IHeatingSystemRepository Repository { get; }
    private IValidator<LlcMatchingBuildArgs> Validator { get; }

    private ILlcMatchingCalculator MatchingCalculator { get; }
    private IReportGenerator ReportGenerator { get; }
    private IDataExporter DataExporter { get; }
    private int? HeatingSystemId { get; set; }


    private async Task TryExecuteCalculateCommand()
    {
        _cts = new CancellationTokenSource();
        var cancellationToken = _cts.Token;
        CalculationInProgress = true;

        HeatingSystem heatingSystem;

        try
        {
            heatingSystem = await GetHeatingSystem();
        }
        catch (ArgumentNullException)
        {
            DisplayValidationErrors("heating system was not selected");
            return;
        }

        // copy data to ensure that user does not modify parameters in middle of the validation which could cause
        // invalidation of the data sent for the further calculation
        var parameters = (LlcMatchingParameters)InputParameters.Clone();

        try
        {
            await ValidateMatchingArguments(parameters, heatingSystem, cancellationToken);
            MatchingResult = await Calculate(parameters, heatingSystem, cancellationToken);
            IsResultCalculated = true;
            UpdateCharts();
        }
        catch (OperationCanceledException)
        {
            Logger.LogDebug("calculation task has been cancelled");
        }
        catch (ValidationException e)
        {
            var errorBuilder = FormatValidationErrors(e.Errors);
            DisplayValidationErrors(errorBuilder);
        }
        catch (SolutionNotFoundException e)
        {
            DisplaySolutionNotFoundError(e);
        }
        finally
        {
            CalculationInProgress = false;
        }
    }


    private async Task<HeatingSystem> GetHeatingSystem()
    {
        ArgumentNullException.ThrowIfNull(HeatingSystemId);

        var heatingSystem = await Repository.Get((int)HeatingSystemId);
        return heatingSystem!;
    }

    private Task ValidateMatchingArguments(LlcMatchingParameters parameters, HeatingSystem heatingSystem,
        CancellationToken cancellationToken)
    {
        var args = new LlcMatchingBuildArgs
            { Parameter = parameters, HeatingSystem = heatingSystem };

        cancellationToken.ThrowIfCancellationRequested();
        return Task.Run(async () => await Validator.ValidateAndThrowAsync(args, cancellationToken), cancellationToken);
    }

    private Task<LlcMatchingResultSummary> Calculate(LlcMatchingParameters parameters, HeatingSystem heatingSystem,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.Run(() => MatchingCalculator.Calculate(parameters, heatingSystem), cancellationToken);
    }

    private void UpdateCharts()
    {
        ArgumentNullException.ThrowIfNull(MatchingResult);

        EventAggregator.GetEvent<CalculationFinishedEvent>()
            .Publish(MatchingResult.Points
                .Cast<MatchingResultPoint>()
                .ToArray());
    }

    private static string FormatValidationErrors(IEnumerable<ValidationFailure> validationFailures)
    {
        var errorBuilder = new StringBuilder();
        foreach (var validationError in validationFailures) errorBuilder.Append(validationError.ErrorMessage);
        return errorBuilder.ToString();
    }

    private void DisplayValidationErrors(string errors)
    {
        ToastService.Show(errors);
        Logger.LogInformation("model has failed validation for fallowing reasons: {Errors}", errors);
    }

    private void DisplaySolutionNotFoundError(SolutionNotFoundException e)
    {
        const string message = "no solution was found for given parameter";
        Logger.LogWarning($"{message}: reason: {{reason}}", e.Message);
        ToastService.Show(message);
    }

    private async Task ExecuteExportDataCommand()
    {
        ArgumentNullException.ThrowIfNull(MatchingResult);

        SetSaveDialogParameters();
        var result = ShowSaveDialog();
        if (result is false)
            return;

        var fileName = GetFileNameFromDialog();

        try
        {
            var report = GenerateReport(MatchingResult);
            await ExportReport(fileName, report);
        }
        finally
        {
            SaveFileDialog.Reset();
        }
    }

    private void SetSaveDialogParameters()
    {
        SaveFileDialog.FileName = "doc";
        SaveFileDialog.DefaultExt = FileExtension.Csv();
        var filter = new DialogFilterCollection();
        filter.AddFilterRow(DialogCommonFilters.CsvFiles);
        SaveFileDialog.Filter = filter;
    }

    private bool? ShowSaveDialog()
    {
        return SaveFileDialog.ShowDialog();
    }

    private string GetFileNameFromDialog()
    {
        return SaveFileDialog.FileName;
    }

    private DataTable GenerateReport(MatchingResultSummaryBase matchingResultSummary)
    {
        return ReportGenerator.CreateSheetReport(matchingResultSummary);
    }

    private async Task ExportReport(string fileName, DataTable report)
    {
        await DataExporter.ExportToCsv(fileName, report);
        Logger.LogInformation("created new csv file at {Path}", fileName);
        ToastService.Show($"exported file {fileName}");
    }

    private Task ExecuteCancelCalculateCommand()
    {
        return Task.Run(() =>
        {
            Logger.LogDebug("requested task cancellation");
            _cts?.Cancel();
            CalculationInProgress = false;
        });
    }

    private void HeatingSystemSelectionChangedHandler(HeatingSystemNameDisplayModel? obj)
    {
        HeatingSystemId = obj?.Id;
    }
}
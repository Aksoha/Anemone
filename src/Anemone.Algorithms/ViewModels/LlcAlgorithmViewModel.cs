using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Anemone.Algorithms.Builders;
using Anemone.Algorithms.Matching;
using Anemone.Algorithms.Models;
using Anemone.Algorithms.Report;
using Anemone.Core;
using Anemone.Core.Dialogs;
using Anemone.Repository.HeatingSystemData;
using FluentValidation;
using FluentValidation.Results;
using MatchingAlgorithm;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Events;
using HeatingSystem = Anemone.Repository.HeatingSystemData.HeatingSystem;

namespace Anemone.Algorithms.ViewModels;

public class LlcAlgorithmViewModel : ViewModelBase
{
    private bool _calculationInProgress;
    private CancellationTokenSource? _cancellationToken;
    private bool _isResultCalculated;
    private LlcMatchingResult? _results;


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
        
        CalculateCommand = new ActionCommandAsync(TryExecuteCalculateCommand);
        ExportDataCommand =
            new DelegateCommand(async () => await ExecuteExportDataCommand()).ObservesCanExecute(() =>
                CanExecuteExportDataCommand);

        EventAggregator.GetEvent<HeatingSystemSelectionChangedEvent>().Subscribe(SelectionChanged);
    }

    private void ExecuteViewDetailedResultsCommand()
    {
        throw new NotImplementedException();
    }

    private async Task ExecuteExportDataCommand()
    {
        SaveFileDialog.FileName = "doc";
        SaveFileDialog.DefaultExt = DialogFilterExtension.Csv();
        var filter = new DialogFilterCollection();
        filter.AddFilterRow(DialogCommonFilters.CsvFiles);
        SaveFileDialog.Filter = filter;

        var result = SaveFileDialog.ShowDialog();
        if (result is false)
            return;

        var fileName = SaveFileDialog.FileName;

        try
        {
            ArgumentNullException.ThrowIfNull(Results);
            var dataToExport = ReportGenerator.Generate(Results);
            await DataExporter.ExportToCsv(fileName, dataToExport);
            Logger.LogInformation("created new csv file at {Path}", fileName);
            ToastService.Show("exported file");
        }
        finally
        {
            SaveFileDialog.Reset();
        }
    }

    private IHeatingSystemRepository Repository { get; }
    private IValidator<LlcMatchingBuildArgs> Validator { get; }
    private IToastService ToastService { get; }
    private ILogger<LlcAlgorithmViewModel> Logger { get; }
    private IEventAggregator EventAggregator { get; }
    
    private ILlcMatchingCalculator MatchingCalculator { get; }
    private IReportGenerator ReportGenerator { get; }
    private IDataExporter DataExporter { get; }
    private ISaveFileDialog SaveFileDialog { get; }
    
    
    public LlcMatchingParameters MatchingParameters { get; } = new();

    public LlcMatchingResult? Results
    {
        get => _results;
        private set
        {
            if (SetProperty(ref _results, value))
                RaisePropertyChanged(nameof(CanExecuteExportDataCommand));
        }
    }


    public bool IsResultCalculated
    {
        get => _isResultCalculated;
        set => SetProperty(ref _isResultCalculated, value);
    }

    private bool CanExecuteExportDataCommand => Results is not null;

    private HeatingSystemNameDisplayModel? HeatingSystemListName { get; set; }
    
    public ICommand CalculateCommand { get; }
    public ICommand ExportDataCommand { get; }

    public bool CalculationInProgress
    {
        get => _calculationInProgress;
        set
        {
            if (SetProperty(ref _calculationInProgress, value))
                RaisePropertyChanged(nameof(CalculationButtonText));
        }
    }

    public string CalculationButtonText => CalculationInProgress ? "Cancel" : "Calculate";
    

    private void SelectionChanged(HeatingSystemNameDisplayModel? obj)
    {
        HeatingSystemListName = obj;
    }


    private bool CanExecuteCalculateCommand(HeatingSystem heatingSystem)
    {
        var validationResult = Validator.Validate(new LlcMatchingBuildArgs
            { Parameter = MatchingParameters, HeatingSystem = heatingSystem });
        if (validationResult.IsValid)
            return true;

        var errorBuilder = new StringBuilder();
        foreach (var validationError in validationResult.Errors) errorBuilder.Append(validationError.ErrorMessage);

        ToastService.Show(errorBuilder.ToString());
        Logger.LogInformation(errorBuilder.ToString());

        return false;
    }

    private Task Calculate(HeatingSystem heatingSystem)
    {
        try
        {
            Results = MatchingCalculator.Calculate(MatchingParameters, heatingSystem);
        }
        catch (SolutionNotFoundException e)
        {
            const string message = "no solution was found for given parameter";
            Logger.LogWarning($"{message}: reason: {{reason}}", e.Message);
            ToastService.Show(message);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "an error has occured while performing llc calculation");
        }

        return Task.CompletedTask;
    }

    private void UpdateCharts()
    {
        EventAggregator.GetEvent<CalculationFinishedEvent>().Publish(Results.Points.ToArray());
    }


    private async Task TryExecuteCalculateCommand()
    {
        var heatingSystem = await GetHeatingSystem();

        if (ValidateAndDisplayErrors(heatingSystem) is false)
            return;

        await RunCalculationTask(heatingSystem);
    }

    private async Task RunCalculationTask(HeatingSystem heatingSystem)
    {
        Logger.LogDebug("starting llc calculation");
        CalculationInProgress = true;
        _cancellationToken = new CancellationTokenSource();
        await Task.Run(() => Calculate(heatingSystem), _cancellationToken.Token);
        UpdateCharts();
        CalculationInProgress = false;
        IsResultCalculated = true;
        Logger.LogDebug("finished llc calculation");
    }


    private bool ValidateAndDisplayErrors([NotNullWhen(true)] HeatingSystem? heatingSystem)
    {
        if (heatingSystem is null)
        {
            DisplayValidationErrors("select heating system");
            return false;
        }

        var validationResult = ValidateBuildArgs(new LlcMatchingBuildArgs
            { Parameter = MatchingParameters, HeatingSystem = heatingSystem });
        if (validationResult.IsValid)
            return true;

        var errorBuilder = FormatValidationErrors(validationResult);
        DisplayValidationErrors(errorBuilder);

        return false;
    }

    private ValidationResult ValidateBuildArgs(LlcMatchingBuildArgs args)
    {
        return Validator.Validate(args);
    }

    private static string FormatValidationErrors(ValidationResult validationResult)
    {
        var errorBuilder = new StringBuilder();
        foreach (var validationError in validationResult.Errors) errorBuilder.Append(validationError.ErrorMessage);
        return errorBuilder.ToString();
    }

    private async ValueTask<HeatingSystem?> GetHeatingSystem()
    {
        if (HeatingSystemListName is null) return null;

        var heatingSystem = await Repository.Get(HeatingSystemListName.Id);
        return heatingSystem;
    }

    private void CancelCalculation()
    {
        _cancellationToken?.Cancel();
        CalculationInProgress = false;
    }

    private void DisplayValidationErrors(string errors)
    {
        ToastService.Show(errors);
        Logger.LogInformation("model has failed validation for fallowing reasons: {Errors}", errors);
    }
}
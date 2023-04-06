using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Anemone.Algorithms.Builders;
using Anemone.Algorithms.Matching;
using Anemone.Algorithms.Models;
using Anemone.Algorithms.Report;
using Anemone.Core;
using Anemone.Repository.HeatingSystemData;
using FluentValidation;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MatchingAlgorithm;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Events;
using SkiaSharp;
using HeatingSystem = Anemone.Repository.HeatingSystemData.HeatingSystem;

namespace Anemone.Algorithms.ViewModels;

public class LlcAlgorithmViewModel : ViewModelBase
{
    private bool _calculationInProgress;
    private CancellationTokenSource? _cancellationToken;
    private LlcMatchingResult? _results;
    private bool _isResultCalculated;


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
        ViewDetailedResultsCommand =
            new DelegateCommand(ExecuteViewDetailedResultsCommand).ObservesCanExecute(() =>
                CanExecuteExportDataCommand);
        CalculateCommand = new ActionCommandAsync(ExecuteCalculateCommand);
        ExportDataCommand =
            new DelegateCommand(async () => await ExecuteExportDataCommand()).ObservesCanExecute(() =>
                CanExecuteExportDataCommand);

        EventAggregator.GetEvent<HeatingSystemSelectionChangedEvent>().Subscribe(SelectionChanged);

        InitializeChartProperties();
    }

    private void ExecuteViewDetailedResultsCommand()
    {
        throw new NotImplementedException();
    }

    private async Task ExecuteExportDataCommand()
    {
        SaveFileDialog.FileName = "doc";
        SaveFileDialog.DefaultExt = ".csv";
        SaveFileDialog.Filter = "csv files (.csv)|*.csv";
        
        var result = SaveFileDialog.ShowDialog();
        if(result is false)
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
    public LlcMatchingParameter Parameter { get; } = new();

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

    private HeatingSystemListName? HeatingSystemListName { get; set; }

    public ICommand ViewDetailedResultsCommand { get; }
    public ICommand CalculateCommand { get; }
    public ICommand ExportDataCommand { get; }

    public bool CalculationInProgress
    {
        get => _calculationInProgress;
        set
        {
            if (SetProperty(ref _calculationInProgress, value))
                RaisePropertyChanged(nameof(ButtonText));
        }
    }

    public string ButtonText => CalculationInProgress ? "Cancel" : "Calculate";


    public ObservableCollection<ISeries> PowerChartSeriesCollection { get; set; }
    public ObservableCollection<ISeries> FrequencyChartSeriesCollection { get; set; }
    public ObservableCollection<ISeries> InductanceChartSeriesCollection { get; set; }

    private LineSeries<LlcMatchingResultPoint> PowerChartCurrentSeries =>
        (LineSeries<LlcMatchingResultPoint>)PowerChartSeriesCollection[1];

    private LineSeries<LlcMatchingResultPoint> FrequencyChartCurrentSeries =>
        (LineSeries<LlcMatchingResultPoint>)FrequencyChartSeriesCollection[1];

    private LineSeries<LlcMatchingResultPoint> InductanceChartCurrentSeries =>
        (LineSeries<LlcMatchingResultPoint>)InductanceChartSeriesCollection[1];

    private LineSeries<LlcMatchingResultPoint> PowerChartPreviousSeries =>
        (LineSeries<LlcMatchingResultPoint>)PowerChartSeriesCollection[0];

    private LineSeries<LlcMatchingResultPoint> FrequencyChartPreviousSeries =>
        (LineSeries<LlcMatchingResultPoint>)FrequencyChartSeriesCollection[0];

    private LineSeries<LlcMatchingResultPoint> InductanceChartPreviousSeries =>
        (LineSeries<LlcMatchingResultPoint>)InductanceChartSeriesCollection[0];

    public Axis[] XAxesCollection { get; set; }
    public Axis[] PowerChartYAxesCollection { get; set; }
    public Axis[] FrequencyChartYAxesCollection { get; set; }
    public Axis[] InductanceChartYAxesCollection { get; set; }

    private Axis PowerChartYAxes => PowerChartYAxesCollection[0];
    private Axis FrequencyChartYAxes => FrequencyChartYAxesCollection[0];
    private Axis InductanceChartYAxes => InductanceChartYAxesCollection[0];

    [MemberNotNull(nameof(XAxesCollection))]
    [MemberNotNull(nameof(PowerChartYAxesCollection))]
    [MemberNotNull(nameof(FrequencyChartYAxesCollection))]
    [MemberNotNull(nameof(InductanceChartYAxesCollection))]
    [MemberNotNull(nameof(PowerChartSeriesCollection))]
    [MemberNotNull(nameof(FrequencyChartSeriesCollection))]
    [MemberNotNull(nameof(InductanceChartSeriesCollection))]
    private void InitializeChartProperties()
    {
        XAxesCollection = new Axis[] { new ChartAxis { Name = "Temperature [°C]" } };
        PowerChartYAxesCollection = new Axis[] { new ChartAxis { Name = "Power [W]" } };
        FrequencyChartYAxesCollection = new Axis[] { new ChartAxis { Name = "Frequency [Hz]" } };
        InductanceChartYAxesCollection = new Axis[] { new ChartAxis { Name = "Inductance [H]" } };

        var purple = new SKColor(143, 88, 191);
        var purpleDark = new SKColor(81, 46, 112);
        var strokeThickness = 4f;

        PowerChartSeriesCollection = new ObservableCollection<ISeries>
        {
            new LineSeries<LlcMatchingResultPoint>
            {
                Name = "Previous",
                Mapping = (data, point) =>
                {
                    point.PrimaryValue = data.Power;
                    point.SecondaryValue = data.Temperature;
                },
                GeometryStroke = null,
                GeometryFill = null,
                Fill = null,
                Stroke = new SolidColorPaint(purpleDark) { StrokeThickness = strokeThickness },
                TooltipLabelFormatter = chartPoint =>
                    $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue:0.00}"
            },
            new LineSeries<LlcMatchingResultPoint>
            {
                Name = "Current",
                Mapping = (data, point) =>
                {
                    point.PrimaryValue = data.Power;
                    point.SecondaryValue = data.Temperature;
                },
                GeometryStroke = null,
                GeometryFill = null,
                Fill = null,
                Stroke = new SolidColorPaint(purple) { StrokeThickness = strokeThickness },
                TooltipLabelFormatter = chartPoint =>
                    $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue:0.00}"
            },
        };

        FrequencyChartSeriesCollection = new ObservableCollection<ISeries>
        {
            new LineSeries<LlcMatchingResultPoint>
            {
                Name = "Previous",
                Mapping = (data, point) =>
                {
                    point.PrimaryValue = data.Frequency;
                    point.SecondaryValue = data.Temperature;
                },
                GeometryStroke = null,
                GeometryFill = null,
                Fill = null,
                Stroke = new SolidColorPaint(purpleDark) { StrokeThickness = strokeThickness },
                TooltipLabelFormatter = chartPoint =>
                    $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue:0.00}",
            },
            new LineSeries<LlcMatchingResultPoint>
            {
                Name = "Current",
                Mapping = (data, point) =>
                {
                    point.PrimaryValue = data.Frequency;
                    point.SecondaryValue = data.Temperature;
                },
                GeometryStroke = null,
                GeometryFill = null,
                Fill = null,
                Stroke = new SolidColorPaint(purple) { StrokeThickness = strokeThickness },
                TooltipLabelFormatter = chartPoint =>
                    $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue:0.00}"
            },
        };

        InductanceChartSeriesCollection = new ObservableCollection<ISeries>
        {
            new LineSeries<LlcMatchingResultPoint>
            {
                Name = "Previous",
                Mapping = (data, point) =>
                {
                    point.PrimaryValue = data.Inductance;
                    point.SecondaryValue = data.Temperature;
                },
                GeometryStroke = null,
                GeometryFill = null,
                Fill = null,
                Stroke = new SolidColorPaint(purpleDark) { StrokeThickness = strokeThickness },
                TooltipLabelFormatter = chartPoint =>
                    $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue:0.00E0}"
            },
            new LineSeries<LlcMatchingResultPoint>
            {
                Name = "Current",
                Mapping = (data, point) =>
                {
                    point.PrimaryValue = data.Inductance;
                    point.SecondaryValue = data.Temperature;
                },
                GeometryStroke = null,
                GeometryFill = null,
                Fill = null,
                Stroke = new SolidColorPaint(purple) { StrokeThickness = strokeThickness },
                TooltipLabelFormatter = chartPoint =>
                    $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue:0.00E0}"
            },
        };
    }

    private void SelectionChanged(HeatingSystemListName? obj)
    {
        HeatingSystemListName = obj;
    }


    private bool CanExecuteCalculateCommand(HeatingSystem heatingSystem)
    {
        var validationResult = Validator.Validate(new LlcMatchingBuildArgs
            { Parameter = Parameter, HeatingSystem = heatingSystem });
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
            Results = MatchingCalculator.Calculate(Parameter, heatingSystem);


            PowerChartPreviousSeries.Values = PowerChartCurrentSeries.Values;
            FrequencyChartPreviousSeries.Values = FrequencyChartCurrentSeries.Values;
            InductanceChartPreviousSeries.Values = InductanceChartCurrentSeries.Values;

            PowerChartCurrentSeries.Values = Results.Points;
            FrequencyChartCurrentSeries.Values = Results.Points;
            InductanceChartCurrentSeries.Values = Results.Points;

            PowerChartYAxes.MinLimit = 0;
            PowerChartYAxes.MaxLimit = Parameter.Power * 1.2;
            FrequencyChartYAxes.MinLimit = 0.8 * Parameter.FrequencyMin;
            FrequencyChartYAxes.MaxLimit = 1.2 * Parameter.FrequencyMax;
            InductanceChartYAxes.MinLimit = 0.8 * Parameter.InductanceMin;
            InductanceChartYAxes.MaxLimit = 1.2 * Parameter.InductanceMax;


            IsResultCalculated = true;
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

    private async Task ExecuteCalculateCommand()
    {
        if (CalculationInProgress)
        {
            _cancellationToken?.Cancel();
            CalculationInProgress = !CalculationInProgress;
            return;
        }

        if (HeatingSystemListName is null)
        {
            ToastService.Show("select heating system");
            return;
        }

        var heatingSystem = await Repository.Get(HeatingSystemListName.Id);
        ArgumentNullException.ThrowIfNull(heatingSystem);

        if (CanExecuteCalculateCommand(heatingSystem!) is false)
            return;


        Logger.LogDebug("starting llc calculation");
        CalculationInProgress = true;
        _cancellationToken = new CancellationTokenSource();
        await Task.Run(() => Calculate(heatingSystem), _cancellationToken.Token);
        CalculationInProgress = false;
        Logger.LogDebug("finished llc calculation");
    }
}
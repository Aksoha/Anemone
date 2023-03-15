using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Anemone.Algorithms.Models;
using Anemone.Core;
using FluentValidation;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MatchingAlgorithm;
using MatchingAlgorithm.Llc;
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
    private LlcMatchingResult[]? _results;


    public LlcAlgorithmViewModel(HeatingRepositoryListViewModel heatingRepositoryListViewModel,
        IValidator<LlcAlgorithmParameters> validator, IToastService toastService, ILogger<LlcAlgorithmViewModel> logger,
        IEventAggregator eventAggregator, ILlcMatchingBuilder matchingBuilder)
    {
        HeatingRepositoryListViewModel = heatingRepositoryListViewModel;
        Validator = validator;
        ToastService = toastService;
        Logger = logger;
        EventAggregator = eventAggregator;
        MatchingBuilder = matchingBuilder;
        CalculateCommand = new ActionCommandAsync(ExecuteCalculateCommand);
        ExportDataCommand = new DelegateCommand(async () => await ExecuteExportDataCommand()).ObservesCanExecute(() => CanExecuteExportDataCommand);
        
        EventAggregator.GetEvent<HeatingSystemSelectionChangedEvent>().Subscribe(SelectionChanged);

        InitializeChartProperties();
    }

    private Task ExecuteExportDataCommand()
    {
        throw new NotImplementedException();
    }

    private IValidator<LlcAlgorithmParameters> Validator { get; }
    private IToastService ToastService { get; }
    private ILogger<LlcAlgorithmViewModel> Logger { get; }
    private IEventAggregator EventAggregator { get; }
    private ILlcMatchingBuilder MatchingBuilder { get; }


    public HeatingRepositoryListViewModel HeatingRepositoryListViewModel { get; }
    public LlcAlgorithmParameters Parameters { get; } = new();

    private LlcMatchingResult[]? Results
    {
        get => _results;
        set
        {
            if(SetProperty(ref _results, value))
                RaisePropertyChanged(nameof(CanExecuteExportDataCommand));
        }
    }

    private bool CanExecuteExportDataCommand => Results is not null;
    
    private HeatingSystemListName? HeatingSystemListName { get; set; }
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

    private LineSeries<LlcMatchingResult> PowerChartCurrentSeries =>
        (LineSeries<LlcMatchingResult>)PowerChartSeriesCollection[1];

    private LineSeries<LlcMatchingResult> FrequencyChartCurrentSeries =>
        (LineSeries<LlcMatchingResult>)FrequencyChartSeriesCollection[1];

    private LineSeries<LlcMatchingResult> InductanceChartCurrentSeries =>
        (LineSeries<LlcMatchingResult>)InductanceChartSeriesCollection[1];    
    private LineSeries<LlcMatchingResult> PowerChartPreviousSeries =>
        (LineSeries<LlcMatchingResult>)PowerChartSeriesCollection[0];

    private LineSeries<LlcMatchingResult> FrequencyChartPreviousSeries =>
        (LineSeries<LlcMatchingResult>)FrequencyChartSeriesCollection[0];

    private LineSeries<LlcMatchingResult> InductanceChartPreviousSeries =>
        (LineSeries<LlcMatchingResult>)InductanceChartSeriesCollection[0];

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
            new LineSeries<LlcMatchingResult>
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
                Stroke = new SolidColorPaint(purpleDark) {StrokeThickness = strokeThickness},
                TooltipLabelFormatter = chartPoint => $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue:0.00}"
            },
            new LineSeries<LlcMatchingResult>
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
                Stroke = new SolidColorPaint(purple) {StrokeThickness = strokeThickness},
                TooltipLabelFormatter = chartPoint => $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue:0.00}"
            },
        };

        FrequencyChartSeriesCollection = new ObservableCollection<ISeries>
        {
            new LineSeries<LlcMatchingResult>
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
                Stroke = new SolidColorPaint(purpleDark) {StrokeThickness = strokeThickness},
                TooltipLabelFormatter = chartPoint => $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue:0.00}",
            },
            new LineSeries<LlcMatchingResult>
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
                Stroke = new SolidColorPaint(purple) {StrokeThickness = strokeThickness},
                TooltipLabelFormatter = chartPoint => $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue:0.00}"
            },
        };

        InductanceChartSeriesCollection = new ObservableCollection<ISeries>
        {
            new LineSeries<LlcMatchingResult>
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
                Stroke = new SolidColorPaint(purpleDark) {StrokeThickness = strokeThickness},
                TooltipLabelFormatter = chartPoint => $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue:0.00E0}"
            },
            new LineSeries<LlcMatchingResult>
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
                Stroke = new SolidColorPaint(purple) {StrokeThickness = strokeThickness},
                TooltipLabelFormatter = chartPoint => $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue:0.00E0}"
            },
        };
    }

    private void SelectionChanged(HeatingSystemListName? obj)
    {
        HeatingSystemListName = obj;
    }


    [MemberNotNullWhen(true, nameof(HeatingSystemListName))]
    private bool CanExecuteCalculateCommand()
    {
        if (HeatingSystemListName is null)
        {
            ToastService.Show("select heating system");
            return false;
        }

        var validationResult = Validator.Validate(Parameters);
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
            var matching = MatchingBuilder.Build(Parameters, heatingSystem);
            Results = matching.EnergyMatching().ToArray();

            PowerChartPreviousSeries.Values = PowerChartCurrentSeries.Values;
            FrequencyChartPreviousSeries.Values = FrequencyChartCurrentSeries.Values;
            InductanceChartPreviousSeries.Values = InductanceChartCurrentSeries.Values;

            PowerChartCurrentSeries.Values = Results;
            FrequencyChartCurrentSeries.Values = Results;
            InductanceChartCurrentSeries.Values = Results;

            PowerChartYAxes.MinLimit = 0;
            PowerChartYAxes.MaxLimit = Parameters.Power * 1.2;
            FrequencyChartYAxes.MinLimit = 0.8 * Parameters.FrequencyMin;
            FrequencyChartYAxes.MaxLimit = 1.2 * Parameters.FrequencyMax;
            InductanceChartYAxes.MinLimit = 0.8 * Parameters.InductanceMin;
            InductanceChartYAxes.MaxLimit = 1.2 * Parameters.InductanceMax;

            var estimator = new MatchingEstimator(Results);

            Logger.LogDebug(
                "mean power: {MeanPower}, turn ratio: {TurnRatio}, capacitance: {Capacitance}, maxFrequencyDerivative: {MaxFrequencyDerivative}, maxInductanceDerivative: {MaxInductanceDerivative}, maxPhaseShift: {MaxPhaseShift}",
                estimator.MeanPower.ToString("0"), estimator.TurnRatio.ToString("0"), estimator.Capacitance.ToString("0.00E0"), estimator.MaxFrequencyDerivative.ToString("0.00"),
                estimator.MaxInductanceDerivative.ToString("0.00E0"), estimator.MaxPhaseShift.ToString("0.00E0"));
        }
        catch (SolutionNotFoundException e)
        {
            const string message = "no solution was found for given parameters";
            Logger.LogWarning($"{message}: reason {{reason}}", e.Message);
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

        if (CanExecuteCalculateCommand() is false)
            return;


        var heatingSystem = await HeatingRepositoryListViewModel.Get(HeatingSystemListName);
        ArgumentNullException.ThrowIfNull(heatingSystem);

        Logger.LogDebug("starting llc calculation");
        CalculationInProgress = true;
        _cancellationToken = new CancellationTokenSource();
        await Task.Run(() => Calculate(heatingSystem), _cancellationToken.Token);
        CalculationInProgress = false;
        Logger.LogDebug("finished llc calculation");
    }
}
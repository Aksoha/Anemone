using System.Collections.ObjectModel;
using Anemone.Core;
using Anemone.DataImport.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Extensions.Logging;

namespace Anemone.DataImport.ViewModels;

public class SaveDataViewModel : ViewModelBase
{
    public SaveDataViewModel(ILogger<SaveDataViewModel> logger)
    {
        Logger = logger;

        ResistanceFrequencyChart.Add(
            new LineSeries<HeatingSystemData>
            {
                Values = FrequencyData,
                Mapping = (data, point) =>
                {
                    point.PrimaryValue = data.Resistance;
                    point.SecondaryValue = data.Key;
                },
                GeometryStroke = null,
                GeometryFill = null,
                Fill = null
            });

        InductanceFrequencyChart.Add(
            new LineSeries<HeatingSystemData>
            {
                Values = FrequencyData,
                Mapping = (data, point) =>
                {
                    point.PrimaryValue = data.Inductance;
                    point.SecondaryValue = data.Key;
                },
                GeometryStroke = null,
                GeometryFill = null,
                Fill = null
            });

        ResistanceTemperatureChart.Add(
            new LineSeries<HeatingSystemData>
            {
                Values = TemperatureData,
                Mapping = (data, point) =>
                {
                    point.PrimaryValue = data.Resistance;
                    point.SecondaryValue = data.Key;
                },
                GeometryStroke = null,
                GeometryFill = null,
                Fill = null
            });

        InductanceTemperatureChart.Add(
            new LineSeries<HeatingSystemData>
            {
                Values = TemperatureData,
                Mapping = (data, point) =>
                {
                    point.PrimaryValue = data.Inductance;
                    point.SecondaryValue = data.Key;
                },
                GeometryStroke = null,
                GeometryFill = null,
                Fill = null
            });
    }

    private ILogger<SaveDataViewModel> Logger { get; }
    public ObservableCollection<HeatingSystemData> FrequencyData { get; } = new();
    public ObservableCollection<HeatingSystemData> TemperatureData { get; } = new();

    public ObservableCollection<ISeries> ResistanceFrequencyChart { get; } = new();
    public ObservableCollection<ISeries> InductanceFrequencyChart { get; } = new();
    public ObservableCollection<ISeries> ResistanceTemperatureChart { get; } = new();
    public ObservableCollection<ISeries> InductanceTemperatureChart { get; } = new();
}
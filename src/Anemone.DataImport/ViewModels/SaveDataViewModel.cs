using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Anemone.Core;
using Anemone.DataImport.Models;
using Anemone.Repository;
using Anemone.Repository.HeatingSystemData;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Extensions.Logging;
using Prism.Services.Dialogs;
using IDialogService = Anemone.Core.Dialogs.IDialogService;

namespace Anemone.DataImport.ViewModels;

public class SaveDataViewModel : ViewModelBase
{
    public SaveDataViewModel(ILogger<SaveDataViewModel> logger,
        IRepository<HeatingSystem> repository, IDialogService dialogService, IToastService toastService)
    {
        Logger = logger;
        Repository = repository;
        DialogService = dialogService;
        ToastService = toastService;

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

        Save = ExecuteSaveDataCommand;
    }

    private ILogger<SaveDataViewModel> Logger { get; }
    private IRepository<HeatingSystem> Repository { get; }
    private IDialogService DialogService { get; }
    private IToastService ToastService { get; }
    public ObservableCollection<HeatingSystemData> FrequencyData { get; } = new();
    public ObservableCollection<HeatingSystemData> TemperatureData { get; } = new();

    public ObservableCollection<ISeries> ResistanceFrequencyChart { get; } = new();
    public ObservableCollection<ISeries> InductanceFrequencyChart { get; } = new();
    public ObservableCollection<ISeries> ResistanceTemperatureChart { get; } = new();
    public ObservableCollection<ISeries> InductanceTemperatureChart { get; } = new();

    public Func<Task> Save { get; set; }

    private async Task ExecuteSaveDataCommand()
    {
        var dialogResult = DialogService.ShowTextBoxDialog(string.Empty, "save data");
        if (dialogResult.Result != ButtonResult.OK)
            return;

        var name = dialogResult.Text;

        var data = new HeatingSystem { Name = name, HeatingSystemPoints = new List<HeatingSystemPoint>() };
        foreach (var point in FrequencyData)
            data.HeatingSystemPoints.Add(new HeatingSystemPoint
            {
                Type = HeatingSystemPointType.Frequency, TypeValue = point.Key, Resistance = point.Resistance,
                Inductance = point.Inductance
            });

        foreach (var point in TemperatureData)
            data.HeatingSystemPoints.Add(new HeatingSystemPoint
            {
                Type = HeatingSystemPointType.Temperature, TypeValue = point.Key, Resistance = point.Resistance,
                Inductance = point.Inductance
            });

        await Repository.Create(data);
        ToastService.Show("created data");
    }
}
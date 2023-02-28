using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using Anemone.Core;
using Anemone.DataImport.Models;
using Anemone.DataImport.Views;

namespace Anemone.DataImport.ViewModels;

internal class MapColumnsViewModel : ViewModelBase
{
    private DataTable _displayedSheet = new();
    private bool _isSheetHeaderVisible;
    private Sheet? _selectedSheet;

    /// <summary>
    ///     Indicates whether <see cref="SheetColumnHeaders" /> is being updated.
    /// </summary>
    /// <remarks>
    ///     Should be called before changing column headers to prevent null exception of
    ///     <see cref="ImportColumnInfoModel.ColumnType" /> due to recursive nature of the call.
    /// </remarks>
    private bool _updatingColumnHeaders;

    public MapColumnsViewModel()
    {
        var enums = HeatingSystemColumnMappingValues.GetValues;
        foreach (var value in enums)
        {
            var status = new MappingInformationModel
            {
                MappedValue = value,
                StatusModel = MappingStatusModel.NotAssigned
            };
            ColumnMappingInformation.Add(status);
        }

        SheetColumnHeaders.CollectionChanged += SheetColumnsOnCollectionChanged;
    }

    /// <summary>
    ///     Data that is displayed on the <see cref="MapColumnsView" />.
    /// </summary>
    public DataTable DisplayedSheet
    {
        get => _displayedSheet;
        private set => SetProperty(ref _displayedSheet, value);
    }

    /// <summary>
    ///     Headers associated with the <see cref="DisplayedSheet" />.
    /// </summary>
    public ObservableCollection<ImportColumnInfoModel> SheetColumnHeaders { get; } = new();

    /// <summary>
    ///     Warning/informations about mapping result.
    /// </summary>
    public ObservableCollection<MappingInformationModel> ColumnMappingInformation { get; } = new();

    /// <summary>
    ///     A list of sheets.
    /// </summary>
    public ObservableCollection<Sheet> Sheets { get; } = new();


    /// <summary>
    ///     Currently selected sheet in the combobox.
    /// </summary>
    public Sheet? SelectedSheet
    {
        get => _selectedSheet;
        set
        {
            if (SetProperty(ref _selectedSheet, value))
                ChangeDisplayedSheet();
        }
    }

    /// <summary>
    ///     Indicates whether first row or <see cref="DisplayedSheet" /> should be shown.
    /// </summary>
    public bool IsSheetHeaderVisible
    {
        get => _isSheetHeaderVisible;
        set
        {
            if (SetProperty(ref _isSheetHeaderVisible, value))
                ChangeDisplayedSheet();
        }
    }

    public event EventHandler<HeatingDataEventArgs>? DataChanged;

    private void ChangeDisplayedSheet()
    {
        if (SelectedSheet is null) return;

        var table = SelectedSheet.Set.ToTable();
        SheetColumnHeaders.Clear();

        foreach (DataColumn dataColumn in table.Columns)
            if (IsSheetHeaderVisible)
                SheetColumnHeaders.Add(new ImportColumnInfoModel
                {
                    ColumnName = table.Rows[0][dataColumn.ColumnName].ToString() ?? string.Empty, Column = dataColumn
                });
            else
                SheetColumnHeaders.Add(new ImportColumnInfoModel
                    { ColumnName = string.Empty, Column = dataColumn });

        if (IsSheetHeaderVisible)
            table.Rows.RemoveAt(0);

        DisplayedSheet = table;
        ResetMappingStatuses();
    }

    /// <summary>
    ///     Clear header which have the same value as sender.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void UpdateColumnHeaders(object? sender, PropertyChangedEventArgs e)
    {
        if (_updatingColumnHeaders) return;

        _updatingColumnHeaders = true;
        var obj = (ImportColumnInfoModel)sender!;
        var columnType = obj.ColumnType;
        var itemsToUpdate = SheetColumnHeaders.Where(x => x.ColumnType == columnType).ToList();
        var idx = itemsToUpdate.IndexOf(obj);
        itemsToUpdate.RemoveAt(idx);

        foreach (var itemToUpdate in itemsToUpdate) itemToUpdate.ColumnType = null;


        foreach (var description in ColumnMappingInformation)
        {
            var sCol = SheetColumnHeaders.SingleOrDefault(x => x.ColumnType == description.MappedValue);
            if (sCol is null)
            {
                description.StatusModel = MappingStatusModel.NotAssigned;
                continue;
            }

            SetMappingStatus(description, sCol.Column);
            SetMappingStatus(description, sCol.Column);
        }


        _updatingColumnHeaders = false;

        if (IsMappingValid())
            SendNotification();
    }

    private void SendNotification()
    {
        if (DataChanged is null) return;

        var fData = new List<HeatingSystemData>();
        var tData = new List<HeatingSystemData>();


        var frequency = SheetColumnHeaders.Single(x => x.ColumnType == HeatingSystemColumnMappingModel.Frequency)
            .Column;
        var temperature = SheetColumnHeaders.Single(x => x.ColumnType == HeatingSystemColumnMappingModel.Temperature)
            .Column;

        var resistanceF = SheetColumnHeaders.Single(x => x.ColumnType == HeatingSystemColumnMappingModel.ResistanceF)
            .Column;
        var inductanceF = SheetColumnHeaders.Single(x => x.ColumnType == HeatingSystemColumnMappingModel.InductanceF)
            .Column;
        var resistanceT = SheetColumnHeaders.Single(x => x.ColumnType == HeatingSystemColumnMappingModel.ResistanceT)
            .Column;
        var inductanceT = SheetColumnHeaders.Single(x => x.ColumnType == HeatingSystemColumnMappingModel.InductanceT)
            .Column;


        foreach (DataRow row in DisplayedSheet.Rows)
        {
            if (row[frequency] is double fVal)
                fData.Add(new HeatingSystemData
                    {
                        Key = fVal,
                        Resistance = (double)row[resistanceF],
                        Inductance = (double)row[inductanceF]
                    }
                );

            if (row[temperature] is double tVal)
                tData.Add(new HeatingSystemData
                    {
                        Key = tVal,
                        Resistance = (double)row[resistanceT],
                        Inductance = (double)row[inductanceT]
                    }
                );
        }


        DataChanged.Invoke(this, new HeatingDataEventArgs { FrequencyData = fData, TemperatureData = tData });
    }


    private void SetMappingStatus(MappingInformationModel mappingInformationModel, DataColumn column)
    {
        var fieldsInfo =
            typeof(HeatingSystemColumnMappingModel).GetField(mappingInformationModel.MappedValue.ToString())!;
        var attribute =
            fieldsInfo.GetCustomAttribute(typeof(ChainedValidationAttribute<HeatingSystemColumnMappingModel>)) as
                ChainedValidationAttribute<HeatingSystemColumnMappingModel>;

        var associatedColumns = new List<DataColumn>();
        if (attribute is not null)
            associatedColumns = SheetColumnHeaders
                .Where(x => attribute.ValidateTogetherWith.All(y => y == x.ColumnType))
                .Select(x => x.Column).ToList();


        if ((from DataRow row in DisplayedSheet.Rows
                let thisColumnValue = row[column].ToString()
                let thisColumnValueHasValue = string.IsNullOrWhiteSpace(thisColumnValue) is false
                from associatedColumn in associatedColumns
                let associatedValue = row[associatedColumn].ToString()
                let associatedColumnHasValue = string.IsNullOrWhiteSpace(associatedValue) is false
                where thisColumnValueHasValue != associatedColumnHasValue
                select thisColumnValueHasValue).Any())
        {
            mappingInformationModel.StatusModel = MappingStatusModel.InconsistentData;
            return;
        }

        mappingInformationModel.StatusModel = MappingStatusModel.Ok;
    }

    private void SheetColumnsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
            foreach (ImportColumnInfoModel item in e.OldItems)
                item.PropertyChanged -= UpdateColumnHeaders;

        if (e.NewItems == null) return;
        foreach (ImportColumnInfoModel item in e.NewItems) item.PropertyChanged += UpdateColumnHeaders;
    }


    private void ResetMappingStatuses()
    {
        foreach (var description in ColumnMappingInformation) description.StatusModel = MappingStatusModel.NotAssigned;
    }

    private bool IsMappingValid()
    {
        return ColumnMappingInformation.All(x => x.StatusModel == MappingStatusModel.Ok);
    }
}
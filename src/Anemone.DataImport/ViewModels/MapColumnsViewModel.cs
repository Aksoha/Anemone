using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
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
        }


        _updatingColumnHeaders = false;
    }


    private void SetMappingStatus(MappingInformationModel mappingInformationModel, DataColumn column)
    {
        if ((from DataRow row in DisplayedSheet.Rows select row[column].ToString()).Any(string.IsNullOrWhiteSpace))
        {
            mappingInformationModel.StatusModel = MappingStatusModel.MissingRow;
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
}
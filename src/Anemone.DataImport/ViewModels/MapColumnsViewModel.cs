using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Anemone.Core;
using Anemone.DataImport.Models;

namespace Anemone.DataImport.ViewModels;

internal class MapColumnsViewModel : ViewModelBase
{
    private DataTable _previewData = new();

    public DataTable PreviewData
    {
        get => _previewData;
        set
        {
            if (SetProperty(ref _previewData, value))
                TryMapAllColumns();
        }
    }

    public ObservableCollection<ImportColumnInfoModel> PreviewColumnData { get; } = new();


    public MapColumnsViewModel()
    {
        PreviewColumnData.CollectionChanged += PreviewColumnDataOnCollectionChanged;
    }
    
    private void TryMapAllColumns()
    {
        // TODO: attempt to map HeatingSystemColumnMappingModel to column headers
    }

    /// <summary>
    /// Clear header which have the same value as sender. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void UpdateColumnHeaders(object? sender, PropertyChangedEventArgs e)
    {
       var obj = (ImportColumnInfoModel)sender!;
       var itemsToUpdate = PreviewColumnData.Where(x => x.ColumnType == obj.ColumnType).ToList();
       var idx = itemsToUpdate.IndexOf(obj);
       itemsToUpdate.RemoveAt(idx);

       foreach (var itemToUpdate in itemsToUpdate)
       {
           itemToUpdate.ColumnType = null;
       }
       
    }


    private void PreviewColumnDataOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
            foreach (ImportColumnInfoModel item in e.OldItems)
            {
                item.PropertyChanged -= UpdateColumnHeaders;
            }

        if (e.NewItems == null) return;
        foreach (ImportColumnInfoModel item in e.NewItems)
        {
            item.PropertyChanged += UpdateColumnHeaders;
        }
    }
}
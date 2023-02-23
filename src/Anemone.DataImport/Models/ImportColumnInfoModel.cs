using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;

namespace Anemone.DataImport.Models;

internal class ImportColumnInfoModel : INotifyPropertyChanged
{
    private string _columnName = string.Empty;
    private HeatingSystemColumnMappingModel? _columnType;

    public HeatingSystemColumnMappingModel? ColumnType
    {
        get => _columnType;
        set => SetField(ref _columnType, value);
    }

    public string ColumnName
    {
        get => _columnName;
        set => SetField(ref _columnName, value);
    }

    public required DataColumn Column { get; set; }
    public bool IsVisible { get; set; } = true;
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
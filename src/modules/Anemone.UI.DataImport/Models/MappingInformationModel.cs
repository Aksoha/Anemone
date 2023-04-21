using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using EnumConverter = Anemone.UI.Core.Converters.EnumConverter;

namespace Anemone.UI.DataImport.Models;

public class MappingInformationModel : INotifyPropertyChanged
{
    private HeatingSystemColumnMappingModel _mappedValue;
    private MappingStatusModel _statusModel;

    public HeatingSystemColumnMappingModel MappedValue
    {
        get => _mappedValue;
        set => SetField(ref _mappedValue, value);
    }

    public MappingStatusModel StatusModel
    {
        get => _statusModel;
        set
        {
            if (SetField(ref _statusModel, value)) OnPropertyChanged(nameof(Description));
        }
    }

    public string Description
    {
        get
        {
            return StatusModel switch
            {
                MappingStatusModel.Ok => FormatMappingDescription(MappingStatusModel.Ok),
                MappingStatusModel.MissingRow => FormatMappingDescription(MappingStatusModel.MissingRow),
                MappingStatusModel.NotAssigned => FormatMappingDescription(MappingStatusModel.NotAssigned),
                MappingStatusModel.InconsistentData => FormatMappingDescription(MappingStatusModel.InconsistentData),
                _ => throw new UnreachableException()
            };
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private static string MappingStatusAsString(MappingStatusModel mappingStatusModel)
    {
        var fieldInfo = mappingStatusModel.GetType().GetField(mappingStatusModel.ToString())!;
        var attribute = fieldInfo.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
        return attribute?.Description ?? mappingStatusModel.ToString();
    }

    private string FormatMappingDescription(MappingStatusModel mappingStatusModel)
    {
        var columnName = (string)new EnumConverter().Convert(MappedValue,
            typeof(string), null, CultureInfo.InvariantCulture);
        var mappingDescription = MappingStatusAsString(mappingStatusModel);
        var builder = new StringBuilder();
        builder.Append(columnName);

        if (string.IsNullOrWhiteSpace(mappingDescription)) return builder.ToString();
        builder.Append(" - ");
        builder.Append(mappingDescription);

        return builder.ToString();
    }

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
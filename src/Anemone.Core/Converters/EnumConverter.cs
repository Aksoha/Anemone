using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace Anemone.Core.Converters;

/// <summary>
///     Converts enum to string.
/// </summary>
/// <remarks>Uses <see cref="DisplayAttribute" /> for conversion if declared.</remarks>
public class EnumConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string) return value;

        var enumObj = (Enum)value;
        return GetEnumDescription(enumObj);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }

    private static string GetEnumDescription(Enum enumObj)
    {
        var fieldInfo = enumObj.GetType().GetField(enumObj.ToString())!;
        var attribute = fieldInfo.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
        return attribute?.Name ?? enumObj.ToString();
    }
}
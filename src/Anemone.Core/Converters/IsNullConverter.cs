﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Anemone.Core.Converters;

public class IsNullConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anemone.Core.Components;

// taken from https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit/blob/master/MaterialDesignThemes.Wpf/PackIcon.cs
public class PackIcon : Control
{
    private static readonly Lazy<IDictionary<PackIconKind, string>> DataIndex = new(PackIconDataFactory.Create);

    public static readonly DependencyProperty KindProperty = DependencyProperty.Register(
        nameof(Kind),
        typeof(PackIconKind),
        typeof(PackIcon),
        new PropertyMetadata(default(PackIconKind), KindPropertyChangedCallback));

    public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
        nameof(Data),
        typeof(string),
        typeof(PackIcon),
        new PropertyMetadata(""));

    static PackIcon()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(PackIcon), new FrameworkPropertyMetadata(typeof(PackIcon)));
    }

    public PackIconKind Kind
    {
        get => (PackIconKind)GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    [TypeConverter(typeof(GeometryConverter))]
    public string? Data
    {
        get => (string?)GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    private static void KindPropertyChangedCallback(DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs e)
    {
        ((PackIcon)dependencyObject).UpdateData();
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateData();
    }

    private void UpdateData()
    {
        DataIndex.Value.TryGetValue(Kind, out var data);
        Data = data;
    }
}
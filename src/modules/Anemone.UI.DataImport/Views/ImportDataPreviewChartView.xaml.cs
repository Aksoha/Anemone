using System.Collections.Generic;
using System.Windows;
using Anemone.UI.Core.Charts;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace Anemone.UI.DataImport.Views;

public partial class ImportDataPreviewChartView
{
    public static readonly DependencyProperty SeriesProperty = DependencyProperty.Register(
        nameof(Series),
        typeof(IEnumerable<ISeries>),
        typeof(ImportDataPreviewChartView)
    );


    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title),
        typeof(string),
        typeof(ImportDataPreviewChartView));


    public ImportDataPreviewChartView()
    {
        InitializeComponent();
    }

    public IEnumerable<ISeries> Series
    {
        get => (IEnumerable<ISeries>)GetValue(SeriesProperty);
        set => SetValue(SeriesProperty, value);
    }


    public string? Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public Axis[]? XAxes { get; } = { new ChartAxis() };

    public Axis[]? YAxes { get; } = { new ChartAxis() };
}
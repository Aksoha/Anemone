using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Anemone.Algorithms.Models;
using Anemone.Core;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using Prism.Events;
using SkiaSharp;

namespace Anemone.Algorithms.ViewModels;

public abstract class MatchingChartViewModelBase<TResult> 
    where TResult : MatchingResultPoint
{
    private IEventAggregator EventAggregator { get; }
    public ObservableCollection<ISeries> Series { get; } = new();
    private LineSeries<TResult> CurrentSeries => (LineSeries<TResult>)Series[0];
    private LineSeries<TResult> PreviousSeries => (LineSeries<TResult>)Series[1];
    public Axis[] XAxesCollection { get; private set; }
    public Axis[] YAxesCollection { get; private set; }


    private const float StrokeThickness = 4f;

    protected MatchingChartViewModelBase(IEventAggregator eventAggregator)
    {
        EventAggregator = eventAggregator;
        CreateAxes();

        var purple = new SKColor(143, 88, 191);
        var purpleDark = new SKColor(81, 46, 112);

        Series.Add(CreateSeries("Current", purple));
        Series.Add(CreateSeries("Previous", purpleDark));

        EventAggregator.GetEvent<CalculationFinishedEvent>().Subscribe(CalculationFinishedEventHandler);
    }

    private void CalculationFinishedEventHandler(MatchingResultPoint[] obj)
    {
        var matchingResults =obj.OfType<TResult>();
        UpdateSeries(matchingResults);
    }


    [MemberNotNull(nameof(XAxesCollection))]
    [MemberNotNull(nameof(YAxesCollection))]
    private void CreateAxes()
    {
        var overrides = AxisOverride();
        XAxesCollection = new Axis[] { new ChartAxis { Name = overrides.XAxesName } };
        YAxesCollection = new Axis[] { new ChartAxis { Name = overrides.YAxesName } };
    }

    private LineSeries<TResult> CreateSeries(string seriesName, SKColor stroke)
    {
        return new LineSeries<TResult>
        {
            Name = seriesName,
            Mapping = PointsMapping,
            GeometryStroke = null,
            GeometryFill = null,
            Fill = null,
            Stroke = new SolidColorPaint(stroke) { StrokeThickness = StrokeThickness },
            TooltipLabelFormatter = TooltipLabelFormatter
        };
    }

    protected abstract void PointsMapping(TResult arg1, ChartPoint arg2);


    protected abstract string TooltipLabelFormatter(
        ChartPoint<TResult, BezierPoint<CircleGeometry>, LabelGeometry> chartPoint);

    protected abstract ChartAxisOverrides AxisOverride();
    
    
    private void UpdateSeries(IEnumerable<TResult> points)
    {
        PreviousSeries.Values = CurrentSeries.Values;
        CurrentSeries.Values = points;
    }
}
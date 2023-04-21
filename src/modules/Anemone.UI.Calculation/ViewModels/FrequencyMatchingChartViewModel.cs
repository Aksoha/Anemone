using Anemone.Core.EnergyMatching.Results;
using Anemone.UI.Calculation.Models;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using Prism.Events;

namespace Anemone.UI.Calculation.ViewModels;

public class FrequencyMatchingChartViewModel : MatchingChartViewModelBase<MatchingResultPoint>
{
    public FrequencyMatchingChartViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
    {
    }

    protected override void PointsMapping(MatchingResultPoint matchingResultPoint, ChartPoint chartPoint)
    {
        chartPoint.PrimaryValue = matchingResultPoint.Frequency;
        chartPoint.SecondaryValue = matchingResultPoint.Temperature;
    }

    protected override string TooltipLabelFormatter(
        ChartPoint<MatchingResultPoint, BezierPoint<CircleGeometry>, LabelGeometry> chartPoint)
    {
        return $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue:0.00}";
    }

    protected override ChartAxisOverrides AxisOverride()
    {
        return new ChartAxisOverrides { XAxesName = "Temperature [°C]", YAxesName = "Frequency [Hz]" };
    }
}
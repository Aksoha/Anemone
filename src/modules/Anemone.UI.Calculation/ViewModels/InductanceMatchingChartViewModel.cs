using Anemone.Core.EnergyMatching.Results;
using Anemone.UI.Calculation.Models;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using Prism.Events;

namespace Anemone.UI.Calculation.ViewModels;

public class InductanceMatchingChartViewModel : MatchingChartViewModelBase<LlcMatchingResultPoint>
{
    public InductanceMatchingChartViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
    {
    }

    protected override void PointsMapping(LlcMatchingResultPoint matchingResultPoint, ChartPoint chartPoint)
    {
        chartPoint.PrimaryValue = matchingResultPoint.Inductance;
        chartPoint.SecondaryValue = matchingResultPoint.Temperature;
    }

    protected override string TooltipLabelFormatter(
        ChartPoint<LlcMatchingResultPoint, BezierPoint<CircleGeometry>, LabelGeometry> chartPoint)
    {
        return $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue:0.00E0}";
    }

    protected override ChartAxisOverrides AxisOverride()
    {
        return new ChartAxisOverrides { XAxesName = "Temperature [°C]", YAxesName = "Inductance [H]" };
    }
}
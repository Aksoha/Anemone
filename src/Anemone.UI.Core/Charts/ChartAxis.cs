using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace Anemone.UI.Core.Charts;

/// <summary>
/// <see cref="Axis"/> with predefined color scheme that matches dark theme.
/// </summary>
public class ChartAxis : Axis
{
    public ChartAxis()
    {
        var whitePaint = new SKColor(255, 255, 255);
        LabelsPaint = new SolidColorPaint(whitePaint);
        SeparatorsPaint = new SolidColorPaint(new SKColor(255, 255, 255, 33));
        NamePaint = new SolidColorPaint(whitePaint);
    }
}
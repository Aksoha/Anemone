using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace Anemone.DataImport.Models;

public class CustomAxis : Axis
{
    public CustomAxis()
    {
        var whitePaint = new SKColor(255, 255, 255);
        LabelsPaint = new SolidColorPaint(whitePaint);
        SeparatorsPaint = new SolidColorPaint(whitePaint);
        NamePaint = new SolidColorPaint(whitePaint);
    }
}
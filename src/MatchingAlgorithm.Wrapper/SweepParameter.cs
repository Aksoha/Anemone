using System.Runtime.InteropServices;

namespace MatchingAlgorithm.Wrapper;

[StructLayout(LayoutKind.Sequential)]
public class SweepParameter
{
    public double Min;
    public double Max;
    public double StepSize;

    public int Length()
    {
        var min = Math.Min(Min, Max);
        var max = Math.Max(Min, Max);
        var diff = max - min;
        return (int)Math.Floor(diff / StepSize);

    }
}
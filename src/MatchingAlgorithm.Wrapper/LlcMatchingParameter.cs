using System.Runtime.InteropServices;

namespace MatchingAlgorithm.Wrapper;

[StructLayout(LayoutKind.Sequential)]
public struct LlcMatchingParameter
{
    public SweepParameter Frequency;
    public SweepParameter Temperature;
    public SweepParameter TurnRatio;
    public double VoltageLimit;
    public double CurrentLimit;
    public double ExpectedPower;
    public SweepParameter Inductance;
    public SweepParameter Capacitance;
}
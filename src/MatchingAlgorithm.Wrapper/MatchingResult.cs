using System.Runtime.InteropServices;

namespace MatchingAlgorithm.Wrapper;

[StructLayout(LayoutKind.Sequential)]
public struct MatchingResult
{
    public double Resistance;
    public double Reactance;
    public double Impedance;
    public double Frequency;
    public double Voltage;
    public double Current;
    public double Power;
    public double TurnRatio;
}
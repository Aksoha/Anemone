using System.Runtime.InteropServices;

namespace MatchingAlgorithm.Wrapper;

[StructLayout(LayoutKind.Sequential)]
public struct HeatingSystemData
{
    public double Key;
    public double Resistance;
    public double Inductance;
}
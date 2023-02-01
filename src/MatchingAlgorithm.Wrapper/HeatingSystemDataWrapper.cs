using System.Runtime.InteropServices;

namespace MatchingAlgorithm.Wrapper;

[StructLayout(LayoutKind.Sequential)]
internal struct HeatingSystemDataWrapper
{
    public double Key;
    public double Resistance;
    public double Inductance;
}
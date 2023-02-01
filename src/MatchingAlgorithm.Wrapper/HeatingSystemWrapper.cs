using System.Runtime.InteropServices;

namespace MatchingAlgorithm.Wrapper;

internal static class HeatingSystemWrapper
{
    [DllImport(ExportNames.LibraryName)]
    public static extern nint CreateHeatingSystem(HeatingSystemDataWrapper[] frequency, HeatingSystemDataWrapper[] temperature,
        long frequencyLength, long temperatureLength);

    [DllImport(ExportNames.LibraryName)]
    public static extern void DisposeHeatingSystem(nint heatingSystem);

    [DllImport(ExportNames.LibraryName)]
    public static extern double Resistance(nint heatingSystem, double frequency, double resistance);
    
    [DllImport(ExportNames.LibraryName)]
    public static extern double Inductance(nint heatingSystem, double frequency, double resistance);
    
    [DllImport(ExportNames.LibraryName)]
    public static extern double Impedance(nint heatingSystem, double frequency, double resistance);
}
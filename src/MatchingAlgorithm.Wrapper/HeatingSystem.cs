using System.Runtime.InteropServices;

namespace MatchingAlgorithm.Wrapper;

public sealed class HeatingSystem : IDisposable
{
    private bool _isDisposed;

    public HeatingSystem(IEnumerable<HeatingSystemData> frequency, IEnumerable<HeatingSystemData> temperature)
    {
        Frequency = frequency.ToArray();
        Temperature = temperature.ToArray();
        HeatingSystemPtr =
            HeatingSystem_Create(Frequency, Temperature, Frequency.Length, Temperature.Length);
    }

    internal nint HeatingSystemPtr { get; private set; }

    private HeatingSystemData[] Frequency { get; }
    private HeatingSystemData[] Temperature { get; }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
        _isDisposed = true;
    }

    public double Resistance(double frequency, double temperature)
    {
        ThrowIfDoesntExist(Frequency, frequency);
        ThrowIfDoesntExist(Temperature, temperature);
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(HeatingSystemPtr));

        return HeatingSystem_Resistance(HeatingSystemPtr, frequency, temperature);
    }

    public double Inductance(double frequency, double temperature)
    {
        ThrowIfDoesntExist(Frequency, frequency);
        ThrowIfDoesntExist(Temperature, temperature);
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(HeatingSystemPtr));

        return HeatingSystem_Inductance(HeatingSystemPtr, frequency, temperature);
    }


    public double Impedance(double frequency, double temperature)
    {
        ThrowIfDoesntExist(Frequency, frequency);
        ThrowIfDoesntExist(Temperature, temperature);
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(HeatingSystemPtr));

        return HeatingSystem_Impedance(HeatingSystemPtr, frequency, temperature);
    }

    private void ThrowIfDoesntExist(IEnumerable<HeatingSystemData> property, double value)
    {
        if (property.Any(x => x.Key.Equals(value)) is false)
            throw new ArgumentOutOfRangeException($"the value {value} was not found in the collection");
    }

    private void ReleaseUnmanagedResources()
    {
        HeatingSystem_Dispose(HeatingSystemPtr);
        HeatingSystemPtr = nint.Zero;
    }

    ~HeatingSystem()
    {
        ReleaseUnmanagedResources();
    }


    [DllImport(ExportNames.LibraryName)]
    private static extern nint HeatingSystem_Create(HeatingSystemData[] frequency, HeatingSystemData[] temperature,
        long frequencyLength, long temperatureLength);

    [DllImport(ExportNames.LibraryName)]
    private static extern void HeatingSystem_Dispose(nint heatingSystem);

    [DllImport(ExportNames.LibraryName)]
    private static extern double HeatingSystem_Resistance(nint heatingSystem, double frequency, double resistance);

    [DllImport(ExportNames.LibraryName)]
    private static extern double HeatingSystem_Inductance(nint heatingSystem, double frequency, double resistance);

    [DllImport(ExportNames.LibraryName)]
    private static extern double HeatingSystem_Impedance(nint heatingSystem, double frequency, double resistance);
}
using System.Runtime.InteropServices;

namespace MatchingAlgorithm.Wrapper;

public sealed class LlcTopology : IDisposable
{
    private double _capacitance;
    private double _inductance;
    private bool _isDisposed;

    public LlcTopology(HeatingSystem heatingSystem)
    {
        HeatingSystem = heatingSystem;
        LlcTopologyPtr = LlcTopology_Create(HeatingSystem.HeatingSystemPtr, 0, 0);
    }

    private HeatingSystem HeatingSystem { get; }
    internal nint LlcTopologyPtr { get; private set; }

    public double Inductance
    {
        get => _inductance;
        set
        {
            _inductance = value;
            LlcTopology_SetInductance(LlcTopologyPtr, _inductance);
        }
    }

    public double Capacitance
    {
        get => _capacitance;
        set
        {
            _capacitance = value;
            LlcTopology_SetCapacitance(LlcTopologyPtr, _capacitance);
        }
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    public double Resistance(double frequency, double temperature)
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(LlcTopologyPtr));

        return LlcTopology_Resistance(LlcTopologyPtr, frequency, temperature);
    }

    public double Reactance(double frequency, double temperature)
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(LlcTopologyPtr));

        return LlcTopology_Reactance(LlcTopologyPtr, frequency, temperature);
    }

    public double Impedance(double frequency, double temperature)
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(LlcTopologyPtr));

        return LlcTopology_Impedance(LlcTopologyPtr, frequency, temperature);
    }

    public double ParallelReactance(double frequency, double temperature)
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(LlcTopologyPtr));

        return LlcTopology_ParallelReactance(LlcTopologyPtr, frequency, temperature);
    }

    private void ReleaseUnmanagedResources()
    {
        LlcTopology_Dispose(LlcTopologyPtr);
        LlcTopologyPtr = nint.Zero;
        _isDisposed = true;
    }

    ~LlcTopology()
    {
        ReleaseUnmanagedResources();
    }

    [DllImport(ExportNames.LibraryName)]
    private static extern nint LlcTopology_Create(nint heatingSystem, double inductance, double capacitance);

    [DllImport(ExportNames.LibraryName)]
    private static extern void LlcTopology_Dispose(nint llc);

    [DllImport(ExportNames.LibraryName)]
    private static extern double LlcTopology_Resistance(nint llc, double frequency, double temperature);

    [DllImport(ExportNames.LibraryName)]
    private static extern double LlcTopology_Reactance(nint llc, double frequency, double temperature);

    [DllImport(ExportNames.LibraryName)]
    private static extern double LlcTopology_Impedance(nint llc, double frequency, double temperature);

    [DllImport(ExportNames.LibraryName)]
    private static extern double LlcTopology_ParallelReactance(nint llc, double frequency, double temperature);

    [DllImport(ExportNames.LibraryName)]
    private static extern void LlcTopology_SetInductance(nint llc, double inductance);

    [DllImport(ExportNames.LibraryName)]
    private static extern void LlcTopology_SetCapacitance(nint llc, double capacitance);
}
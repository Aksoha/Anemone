using System.Runtime.InteropServices;

namespace MatchingAlgorithm.Wrapper;

public sealed class LlcMatching : IDisposable
{
    private readonly LlcTopology _topology;

    public LlcMatching(LlcTopology topology)
    {
        _topology = topology;
    }

    internal nint MatchingPtr { get; private set; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public IEnumerable<LlcMatchingResult> Match(LlcMatchingParameter parameters)
    {
        MatchingPtr = LlcMatching_Create(_topology.LlcTopologyPtr, parameters);
        var length = parameters.Frequency.Length();
        var results = new LlcMatchingResult[length];

        LlcMatching_Match(MatchingPtr, results, length);
        ReleaseUnmanagedResources();

        return results;
    }

    private void ReleaseUnmanagedResources()
    {
        LlcMatching_Dispose(MatchingPtr);
        MatchingPtr = nint.Zero;
    }

    private void Dispose(bool disposing)
    {
        ReleaseUnmanagedResources();
        if (disposing) _topology.Dispose();
    }

    ~LlcMatching()
    {
        Dispose(false);
    }
    
    [DllImport(ExportNames.LibraryName)]
    private static extern nint LlcMatching_Create(nint topology, LlcMatchingParameter parameter);


    [DllImport(ExportNames.LibraryName)]
    private static extern void LlcMatching_Dispose(nint matching);


    [DllImport(ExportNames.LibraryName)]
    private static extern void LlcMatching_Match(nint matching, [Out] LlcMatchingResult[] results, long length);
}
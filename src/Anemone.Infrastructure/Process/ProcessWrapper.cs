using System.Diagnostics;
using Anemone.Core;
using Anemone.Core.Process;

namespace Anemone.Infrastructure.Process;

internal class ProcessWrapper : IProcess
{
    public System.Diagnostics.Process Start(string fileName)
    {
        return System.Diagnostics.Process.Start(fileName);
    }

    public System.Diagnostics.Process Start(string fileName, string arguments)
    {
        return System.Diagnostics.Process.Start(fileName, arguments);
    }

    public System.Diagnostics.Process Start(string fileName, IEnumerable<string> arguments)
    {
        return System.Diagnostics.Process.Start(fileName, arguments);
    }

    public System.Diagnostics.Process? Start(ProcessStartInfo startInfo)
    {
        return System.Diagnostics.Process.Start(startInfo);
    }
}
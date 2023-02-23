using System.Collections.Generic;
using System.Diagnostics;
using Anemone.Core;

namespace Anemone.Services;

public class ProcessWrapper : IProcess
{
    public Process Start(string fileName)
    {
        return Process.Start(fileName);
    }

    public Process Start(string fileName, string arguments)
    {
        return Process.Start(fileName, arguments);
    }

    public Process Start(string fileName, IEnumerable<string> arguments)
    {
        return Process.Start(fileName, arguments);
    }

    public Process? Start(ProcessStartInfo startInfo)
    {
        return Process.Start(startInfo);
    }
}
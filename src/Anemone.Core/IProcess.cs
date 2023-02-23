using System.Collections.Generic;
using System.Diagnostics;

namespace Anemone.Core;

public interface IProcess
{
    Process Start(string fileName);
    Process Start(string fileName, string arguments);
    Process Start(string fileName, IEnumerable<string> arguments);
    Process? Start(ProcessStartInfo startInfo);
}
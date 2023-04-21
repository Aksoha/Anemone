using System.Collections.Generic;
using System.Diagnostics;

namespace Anemone.Core.Process;

public interface IProcess
{
    System.Diagnostics.Process Start(string fileName);
    System.Diagnostics.Process Start(string fileName, string arguments);
    System.Diagnostics.Process Start(string fileName, IEnumerable<string> arguments);
    System.Diagnostics.Process? Start(ProcessStartInfo startInfo);
}
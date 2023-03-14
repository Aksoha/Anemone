using System;
using System.Linq;
using System.Windows;

namespace Anemone.Startup;

public class ApplicationArguments
{
    public bool AttachDebugger { get; set; }
    
    public static ApplicationArguments Parse(StartupEventArgs arguments)
    {
        var output = new ApplicationArguments();
        output.AttachDebugger = arguments.Args.Contains("debug", StringComparer.OrdinalIgnoreCase);

        return output;
    }
}
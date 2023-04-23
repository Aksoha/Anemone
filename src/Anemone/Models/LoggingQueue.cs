using System;
using Serilog.Events;

namespace Anemone.Models;

internal class LoggingQueueItem
{
    public required LogEventLevel LogEventLevel { get; set; }
    public required  string Message  { get; set; }
    public Exception? Exception  { get; set; }
}
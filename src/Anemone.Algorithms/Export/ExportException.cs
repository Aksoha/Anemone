using System;

namespace Anemone.Algorithms.Export;

public class ExportException : Exception
{
    public ExportException()
    {
        
    }

    public ExportException(string? message) : base(message)
    {
    }
}
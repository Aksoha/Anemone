using System;

namespace Anemone.Core.Export;

public class ExportException : Exception
{
    public ExportException()
    {
    }

    public ExportException(string? message) : base(message)
    {
    }
}
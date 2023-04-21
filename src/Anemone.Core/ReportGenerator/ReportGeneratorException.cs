using System;

namespace Anemone.Core.ReportGenerator;

public class ReportGeneratorException : Exception
{
    protected ReportGeneratorException(string message) : base(message)
    {
    }
}
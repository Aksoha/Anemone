using System;

namespace Anemone.Algorithms.Report;

public class ReportGeneratorException : Exception
{
    protected ReportGeneratorException(string message) : base(message)
    {
    }
}
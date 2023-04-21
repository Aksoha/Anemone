using System;

namespace Anemone.Core.ReportGenerator.Table;

public class TableParserException : ReportGeneratorException
{
    public TableParserException(Type objType, int rowId, int colId) : base(FormatException(objType, rowId, colId))
    {
    }

    private static string FormatException(Type objType, int rowId, int colId)
    {
        return $"attempted to convert complex type {objType} to cell at [{rowId},{colId}]";
    }
}
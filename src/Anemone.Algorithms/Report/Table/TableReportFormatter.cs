using System;
using System.Data;
using System.Reflection;
using Anemone.Algorithms.Models;

namespace Anemone.Algorithms.Report.Table;

public abstract class TableReportFormatter<TSummary> where TSummary : MatchingResultSummaryBase
{
    protected readonly TSummary Data;
    protected readonly DataTable Table = new();
    protected readonly DataTableWriter Writer;

    protected TableReportFormatter(TSummary data)
    {
        Data = data;
        Writer = new DataTableWriter(Table);
    }

    public abstract DataTable Format();

    protected static string GetColumnHeaderName(Type type, string propertyName)
    {
        var property = type.GetProperty(propertyName);
        if (property is null)
            throw new ArgumentException($"the {type} does not have a public property \"{propertyName}\"");

        var exportColumnAttribute = property.GetCustomAttribute<ExportColumnAttribute>();
        var headerName = exportColumnAttribute is not null ? exportColumnAttribute.Name : property.Name;
        return headerName;
    }
}
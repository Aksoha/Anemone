using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Anemone.Algorithms.Models;

namespace Anemone.Algorithms.Report.Table;

public class UnspecifiedTableReportFormatter : TableReportFormatter<MatchingResultSummaryBase>
{
    public UnspecifiedTableReportFormatter(MatchingResultSummaryBase data) : base(data)
    {
    }

    public override DataTable Format()
    {
        var properties = Data.GetType().GetProperties();

        AppendHeaderRow(Writer, properties);

        var col = 0;
        foreach (var propertyInfo in properties)
        {
            var propertyVal = propertyInfo.GetValue(Data);
            Writer.WriteColumn(propertyVal, 1, col++);
        }

        return Table;
    }

    private static void AppendHeaderRow(DataTableWriter writer, IEnumerable<PropertyInfo> properties, int startCol = 0)
    {
        foreach (var property in properties)
        {
            var exportColumnAttribute = property.GetCustomAttribute<ExportColumnAttribute>();
            var headerName = exportColumnAttribute is not null ? exportColumnAttribute.Name : property.Name;
            writer.WriteColumn(headerName, 0, startCol++);
        }
    }
}
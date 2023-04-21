using System.Collections;
using System.Data;
using Anemone.Core.ReportGenerator.Table;

namespace Anemone.Infrastructure.Export.Table;

internal class DataTableWriter
{
    private readonly DataTable _table;

    public DataTableWriter(DataTable table)
    {
        _table = table;
    }

    public void WriteColumn(object? obj, int rowId, int colId)
    {
        CreateRequiredColumns(colId);

        if (obj is IEnumerable enumerable)
            WriteCollection(enumerable, rowId, colId);
        else
            WriteCell(obj, rowId, colId);
    }

    private void CreateRequiredColumns(int colId)
    {
        var columns = _table.Columns;

        while (colId >= columns.Count)
            AppendEmptyColumn();
    }


    private void AppendEmptyColumn()
    {
        var col = new DataColumn { DataType = typeof(object) };
        _table.Columns.Add(col);
    }


    private void WriteCollection(IEnumerable enumerable, int startRow, int column)
    {
        // special case, collection was just IEnumerable<Char>
        if (enumerable is string s)
        {
            WriteCell(s, startRow, column);
            return;
        }

        foreach (var item in enumerable)
        {
            var type = item.GetType();
            if (type.IsValueType is false)
                throw new TableParserException(type, startRow, column);

            WriteCell(item, startRow++, column);
        }
    }


    private void WriteCell(object? value, int rowId, int colId)
    {
        var row = GetOrCreateRow(rowId);
        row[colId] = value;
    }


    private DataRow GetOrCreateRow(int rowId)
    {
        var rows = _table.Rows;

        while (rowId >= rows.Count)
            AppendEmptyRow();

        return rows[rowId];
    }


    private void AppendEmptyRow()
    {
        var row = _table.NewRow();
        _table.Rows.Add(row);
    }
}
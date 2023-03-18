using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anemone.Algorithms.Report;

public class DataExporter : IDataExporter
{
    private IFile File { get; }

    public DataExporter(IFile file)
    {
        File = file;
    }
    
    public async Task Export(string filePath, DataTable table)
    {
        if (Path.GetExtension(filePath) != ".csv")
            throw new NotSupportedException("specified file format is not supported");

        await using var fs =  File.Create(filePath);

        var headers = GetHeaders(table);
        await fs.WriteAsync(LineToBytes(headers));
        
        
        foreach (DataRow row in table.Rows)
        {
            var fields = row.ItemArray.Select(field => 
                string.Concat("\"", field?.ToString()?.Replace("\"", "\"\""), "\""));
            var buffer = LineToBytes(fields);
            await fs.WriteAsync(buffer);
        }
        
        fs.Close();
    }

    private static byte[] LineToBytes(IEnumerable<string> fields)
    {
        return Encoding.UTF8.GetBytes(string.Join(",", fields) + "\n");
    }

    private static IEnumerable<string> GetHeaders(DataTable table)
    {
        return (from DataColumn column in table.Columns select column.ColumnName).ToList();
    }
}
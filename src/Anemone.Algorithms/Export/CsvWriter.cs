using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anemone.Algorithms.Export;

public class CsvWriter
{
    public CsvWriter(Stream stream)
    {
        Stream = stream;
    }

    private Stream Stream { get; }

    public async Task WriteRow(DataRow row)
    {
        var line = ConvertToString(row);
        var buffer = Encode(line);
        await Stream.WriteAsync(buffer);
    }

    private static string ConvertToString(DataRow row)
    {
        var line = row.ItemArray.Select(ConcatenateLine);
        var output = string.Join(",", line) + "\n";
        return output;
    }

    private static string ConcatenateLine(object? field)
    {
        return string.Concat("\"", field?.ToString()?.Replace("\"", "\"\""), "\"");
    }


    private static byte[] Encode(string content)
    {
        return Encoding.UTF8.GetBytes(content);
    }
}
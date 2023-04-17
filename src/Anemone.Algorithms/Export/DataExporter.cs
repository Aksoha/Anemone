using System.Data;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Anemone.Algorithms.Export;

public class DataExporter : IDataExporter
{
    public DataExporter(IFile file)
    {
        File = file;
    }

    private IFile File { get; }

    public async Task ExportToCsv(string filePath, DataTable data)
    {
        ThrowIfFormatNotSupported(filePath);

        await using var fs = File.Create(filePath);
        await WriteData(fs, data);
        fs.Close();
    }

    private static void ThrowIfFormatNotSupported(string filePath)
    {
        var extension = Path.GetExtension(filePath);
        if (extension != ".csv")
            throw new ExportFileTypeNotSupportedException(extension)
                { FilePath = filePath, SupportedExtensions = new[] { ".csv" } };
    }

    private static async Task WriteData(Stream stream, DataTable table)
    {
        var writer = new CsvWriter(stream);
        foreach (DataRow row in table.Rows) await writer.WriteRow(row);
    }
}
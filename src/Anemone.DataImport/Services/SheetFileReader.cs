using System.Data;
using System.IO;
using System.IO.Abstractions;
using ExcelDataReader;

namespace Anemone.DataImport.Services;

public class SheetFileReader : ISheetFileReader
{
    private IFile File { get; }

    public SheetFileReader(IFile file)
    {
        File = file;
    }
    
    public DataSet ReadAsDataSet(string path)
    {
        using var stream = File.Open(path, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(stream);
        return reader.AsDataSet();
    }
}
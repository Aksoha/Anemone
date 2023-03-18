using System;
using System.Data;
using System.IO;
using System.IO.Abstractions;
using ExcelDataReader;

namespace Anemone.DataImport.Services;

public class SheetFileReader : ISheetFileReader
{
    public SheetFileReader(IFile file)
    {
        File = file;
    }

    private IFile File { get; }

    public DataSet ReadAsDataSet(string path)
    {
        var extension = Path.GetExtension(path);
        using var stream = File.Open(path, FileMode.Open, FileAccess.Read);

        switch (extension)
        {
            case ".csv":
            {
                using var reader = ExcelReaderFactory.CreateCsvReader(stream);
                return reader.AsDataSet();
            }
            case ".xls":
            case ".xlsx":
            case ".xlsb":
            {
                using var reader = ExcelReaderFactory.CreateReader(stream);
                return reader.AsDataSet();
            }
        }
        throw new NotSupportedException($"{extension} extension is currently not supported");
    }
}
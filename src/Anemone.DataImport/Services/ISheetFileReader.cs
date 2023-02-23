using System.Data;

namespace Anemone.DataImport.Services;

public interface ISheetFileReader
{
    DataSet ReadAsDataSet(string path);
}
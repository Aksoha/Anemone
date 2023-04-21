using System.Data;

namespace Anemone.Core.Import;

public interface IFileReader
{
    DataSet ReadAsDataSet(string path);
}
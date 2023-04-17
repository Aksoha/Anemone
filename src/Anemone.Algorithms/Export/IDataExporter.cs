using System.Data;
using System.Threading.Tasks;

namespace Anemone.Algorithms.Export;

public interface IDataExporter
{
    Task ExportToCsv(string filePath, DataTable data);
}
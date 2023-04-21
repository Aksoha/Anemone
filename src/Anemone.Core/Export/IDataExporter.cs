using System.Data;
using System.Threading.Tasks;

namespace Anemone.Core.Export;

public interface IDataExporter
{
    Task ExportToCsv(string filePath, DataTable data);
}
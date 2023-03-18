using System.Data;
using System.Threading.Tasks;

namespace Anemone.Algorithms.Report;

public interface IDataExporter
{
    Task Export(string filePath, DataTable table);
}
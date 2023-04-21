using System.Data;
using Anemone.Core.EnergyMatching.Results;

namespace Anemone.Core.ReportGenerator;

public interface IReportGenerator
{
    DataTable CreateSheetReport(MatchingResultSummaryBase data);
}
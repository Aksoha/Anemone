using System.Data;
using Anemone.Core.EnergyMatching.Results;
using Anemone.Core.ReportGenerator;
using Anemone.Infrastructure.Export.Table;
using Microsoft.Extensions.Logging;

namespace Anemone.Infrastructure.Export;

public class ReportGenerator : IReportGenerator
{
    public ReportGenerator(ILogger<ReportGenerator> logger)
    {
        Logger = logger;
    }

    private ILogger<ReportGenerator> Logger { get; }

    public DataTable CreateSheetReport(MatchingResultSummaryBase data)
    {
        return data switch
        {
            LlcMatchingResultSummary llc => new LlcTableReportFormatter(llc).Format(),
            _ => GenerateNotRegisteredType(data)
        };
    }

    private DataTable GenerateNotRegisteredType(MatchingResultSummaryBase data)
    {
        var reportFormatter = new UnspecifiedTableReportFormatter(data);
        Logger.LogWarning("generated report for {Type} which had no registered formatter", data.GetType());
        return reportFormatter.Format();
    }
}
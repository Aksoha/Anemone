using System.Collections.Generic;
using System.Data;
using System.Linq;
using Anemone.Algorithms.Models;

namespace Anemone.Algorithms.Report.Table;

public class LlcTableReportFormatter : TableReportFormatter<LlcMatchingResultSummary>
{
    public LlcTableReportFormatter(LlcMatchingResultSummary data) : base(data)
    {
    }

    public override DataTable Format()
    {
        var points = Data.Points;

        var inductance = points.Select(x => x.Inductance);
        var capacitance = points.Select(x => x.Capacitance);
        var resistance = points.Select(x => x.Resistance);
        var reactance = points.Select(x => x.Reactance);
        var impedance = points.Select(x => x.Impedance);
        var frequency = points.Select(x => x.Frequency);
        var temperature = points.Select(x => x.Temperature);
        var voltage = points.Select(x => x.Voltage);
        var current = points.Select(x => x.Current);
        var power = points.Select(x => x.Power);
        var phaseShift = points.Select(x => x.PhaseShift);


        var col = 0;

        WriteColumnWithHeader(nameof(LlcMatchingResultPoint.Inductance), inductance, col++);
        WriteColumnWithHeader(nameof(LlcMatchingResultPoint.Capacitance), capacitance, col++);
        WriteColumnWithHeader(nameof(LlcMatchingResultPoint.Resistance), resistance, col++);
        WriteColumnWithHeader(nameof(LlcMatchingResultPoint.Reactance), reactance, col++);
        WriteColumnWithHeader(nameof(LlcMatchingResultPoint.Impedance), impedance, col++);
        WriteColumnWithHeader(nameof(LlcMatchingResultPoint.Frequency), frequency, col++);
        WriteColumnWithHeader(nameof(LlcMatchingResultPoint.Temperature), temperature, col++);
        WriteColumnWithHeader(nameof(LlcMatchingResultPoint.Voltage), voltage, col++);
        WriteColumnWithHeader(nameof(LlcMatchingResultPoint.Current), current, col++);
        WriteColumnWithHeader(nameof(LlcMatchingResultPoint.Power), power, col++);
        WriteColumnWithHeader(nameof(LlcMatchingResultPoint.PhaseShift), phaseShift, col++);

        return Table;
    }

    private void WriteColumnWithHeader(string propertyName, IEnumerable<double> data, int col)
    {
        Writer.WriteColumn(
            GetColumnHeaderName(typeof(LlcMatchingResultPoint), propertyName), 0, col);
        Writer.WriteColumn(data, 1, col);
    }
}
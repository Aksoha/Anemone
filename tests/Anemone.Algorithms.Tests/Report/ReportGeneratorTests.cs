using System.Data;
using System.Diagnostics.CodeAnalysis;
using Anemone.Algorithms.Models;
using Anemone.Algorithms.Report;
using Bogus;
using Microsoft.Extensions.Logging;
using Moq;

namespace Anemone.Algorithms.Tests.Report;

public class ReportGeneratorTests
{
    private Faker<LlcMatchingResultSummary> _llcSummaryFaker;
    private Faker<LlcMatchingResultPoint> _pointsFaker;
    private Faker<UnregisteredSummary> _unregisteredSummaryFaker;

    public ReportGeneratorTests()
    {
        InitializeFaker();
    }

    private DataTable? Table { get; set; }
    private ReportGenerator? ReportGenerator { get; set; }


    [Fact]
    public void CreateSheetReport_Llc()
    {
        // arrange
        var data = _llcSummaryFaker.Generate();
        var points = data.Points;

        CreateReportGenerator();


        // act
        CreateSheetReport(ReportGenerator, data);


        // assert
        VerifyColumnCount(11, Table);
        var expectedRowCount = data.Points.Length + 1;
        VerifyRowCount(expectedRowCount, Table);
        VerifyFirstRowIsHeader(new[]
        {
            "Inductance",
            "Capacitance",
            "Resistance",
            "Reactance",
            "Impedance",
            "Frequency",
            "Temperature",
            "Voltage",
            "Current",
            "Power",
            "PhaseShift"
        }, Table);


        VerifyColumnContent(Table, points.Select(x => x.Inductance), 1, 0);
        VerifyColumnContent(Table, points.Select(x => x.Capacitance), 1, 1);
        VerifyColumnContent(Table, points.Select(x => x.Resistance), 1, 2);
        VerifyColumnContent(Table, points.Select(x => x.Reactance), 1, 3);
        VerifyColumnContent(Table, points.Select(x => x.Impedance), 1, 4);
        VerifyColumnContent(Table, points.Select(x => x.Frequency), 1, 5);
        VerifyColumnContent(Table, points.Select(x => x.Temperature), 1, 6);
        VerifyColumnContent(Table, points.Select(x => x.Voltage), 1, 7);
        VerifyColumnContent(Table, points.Select(x => x.Current), 1, 8);
        VerifyColumnContent(Table, points.Select(x => x.Power), 1, 9);
        VerifyColumnContent(Table, points.Select(x => x.PhaseShift), 1, 10);
    }

    [Fact]
    public void CreateSheetReport_WhenClassDoesNotHaveFormatter()
    {
        // arrange
        var data = _unregisteredSummaryFaker.Generate();
        CreateReportGenerator();


        // act
        CreateSheetReport(ReportGenerator, data);


        // assert
        VerifyColumnCount(3, Table);
        VerifyRowCount(data.NumberCollection.Length + 1, Table);
        VerifyFirstRowIsHeader(new[] { "StringProperty", "NumberProperty", "NumberCollection" }, Table);
        VerifyColumnContent(Table, new[] { data.StringProperty }, 1, 0);
        VerifyColumnContent(Table, new[] { data.NumberProperty }, 1, 1);
        VerifyColumnContent(Table, data.NumberCollection, 1, 2);
    }


    [Fact]
    public void CreateSheetReport_WhenColumnIsAnnotated()
    {
        // arrange
        var data = new AnnotatedSummary();
        CreateReportGenerator();


        // act
        var table = ReportGenerator.CreateSheetReport(data);


        // assert
        VerifyFirstRowIsHeader(new[] { "custom column name" }, table);
    }


    [MemberNotNull(nameof(_pointsFaker))]
    [MemberNotNull(nameof(_llcSummaryFaker))]
    [MemberNotNull(nameof(_unregisteredSummaryFaker))]
    private void InitializeFaker()
    {
        _pointsFaker = new Faker<LlcMatchingResultPoint>()
            .RuleFor(x => x.Resistance, f => f.Random.Double(20e-3, 40e-3))
            .RuleFor(x => x.Reactance, f => f.Random.Double(0, 2e-7))
            .RuleFor(x => x.Frequency, f => f.Random.Double(20e3, 200e3))
            .RuleFor(x => x.Temperature, f => f.Random.Double(0, 1500))
            .RuleFor(x => x.Voltage, f => f.Random.Double(0, 400))
            .RuleFor(x => x.Current, f => f.Random.Double(0, 200))
            .RuleFor(x => x.Power, f => f.Random.Double(1e3, 100e3))
            .RuleFor(x => x.Capacitance, f => f.Random.Double(20e-6, 100e-6))
            .RuleFor(x => x.Inductance, f => f.Random.Double(1e-8, 15e-8));

        _llcSummaryFaker = new Faker<LlcMatchingResultSummary>()
            .CustomInstantiator(faker => new LlcMatchingResultSummary(_pointsFaker.Generate(20), faker.Random.Int(20)));


        _unregisteredSummaryFaker = new Faker<UnregisteredSummary>()
            .RuleFor(p => p.StringProperty, f => f.Random.Words())
            .RuleFor(p => p.NumberProperty, f => f.Random.Double(100))
            .RuleFor(p => p.NumberCollection, f => f.Make(5, _ => f.Random.Double(100)).ToArray());
    }


    [MemberNotNull(nameof(ReportGenerator))]
    private void CreateReportGenerator()
    {
        var loggerMock = new Mock<ILogger<ReportGenerator>>();
        ReportGenerator = new ReportGenerator(loggerMock.Object);
    }

    [MemberNotNull(nameof(Table))]
    private void CreateSheetReport(IReportGenerator generator, MatchingResultSummaryBase data)
    {
        Table = generator.CreateSheetReport(data);
    }

    private static void VerifyColumnCount(int count, DataTable table)
    {
        Assert.Equal(count, table.Columns.Count);
    }

    private static void VerifyRowCount(int count, DataTable table)
    {
        Assert.Equal(count, table.Rows.Count);
    }

    private static void VerifyFirstRowIsHeader(IReadOnlyList<string> headers, DataTable table)
    {
        var columns = table.Columns;

        Assert.Equal(headers.Count, columns.Count);

        var row = table.Rows[0];

        for (var i = 0; i < headers.Count; i++)
        {
            var header = headers[i];
            var column = row[i];

            Assert.Equal(header, column);
        }
    }

    private static void VerifyColumnContent<T>(DataTable table, IEnumerable<T> content, int rowOffset, int column)
    {
        var rows = table.Rows;
        var contentArray = content.ToArray();

        for (var i = rowOffset; i < contentArray.Length; i++)
        {
            var expectedContent = contentArray[i - rowOffset];
            var actualContent = rows[i][column];
            Assert.Equal(expectedContent, actualContent);
        }
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    private class UnregisteredSummary : MatchingResultSummaryBase
    {
        public string StringProperty { get; set; } = string.Empty;
        public double NumberProperty { get; set; }
        public double[] NumberCollection { get; set; } = Array.Empty<double>();
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private class AnnotatedSummary : MatchingResultSummaryBase
    {
        [ExportColumn("custom column name")] public double AnnotatedProperty { get; set; }
    }
}
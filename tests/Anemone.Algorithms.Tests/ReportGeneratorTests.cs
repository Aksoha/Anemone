using System.Data;
using Anemone.Algorithms.Models;
using Anemone.Algorithms.Report;
using Bogus;

namespace Anemone.Algorithms.Tests;

public class ReportGeneratorTests
{

    private readonly Faker<LlcMatchingResultPoint> _pointsFaker;
    public ReportGeneratorTests()
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
    }
    
    [Fact]
    public void Generate()
    {
        // arrange
        const int pointsCount = 20;
        const int turnRatio = 18;
        var points = _pointsFaker.Generate(pointsCount);
        var data = new LlcMatchingResult(points, turnRatio);
        var generator = new ReportGenerator();
        
        // act
        var result = generator.Generate(data);

        
        // assert
        Assert.Equal(11, result.Columns.Count);
        Assert.Equal(pointsCount, result.Rows.Count);
        var idx = 0;
        foreach (DataRow row in result.Rows)
        {

            var point = points[idx];
            Assert.Equal(point.Resistance, row[nameof(LlcMatchingResultPoint.Resistance)]);
            Assert.Equal(point.Reactance, row[nameof(LlcMatchingResultPoint.Reactance)]);
            Assert.Equal(point.Impedance, row[nameof(LlcMatchingResultPoint.Impedance)]);
            Assert.Equal(point.Frequency, row[nameof(LlcMatchingResultPoint.Frequency)]);
            Assert.Equal(point.Temperature, row[nameof(LlcMatchingResultPoint.Temperature)]);
            Assert.Equal(point.Voltage, row[nameof(LlcMatchingResultPoint.Voltage)]);
            Assert.Equal(point.Current, row[nameof(LlcMatchingResultPoint.Current)]);
            Assert.Equal(point.Power, row[nameof(LlcMatchingResultPoint.Power)]);
            Assert.Equal(point.PhaseShift, row[nameof(LlcMatchingResultPoint.PhaseShift)]);
            Assert.Equal(point.Capacitance, row[nameof(LlcMatchingResultPoint.Capacitance)]);
            Assert.Equal(point.Inductance, row[nameof(LlcMatchingResultPoint.Inductance)]);
            idx++;
        }
    }
}

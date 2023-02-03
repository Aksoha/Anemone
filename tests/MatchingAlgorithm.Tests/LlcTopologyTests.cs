using System.Collections;
using MatchingAlgorithm.Wrapper;
using Xunit.Sdk;

namespace MatchingAlgorithm.Tests;

public class LlcTopologyTests
{
    [Theory]
    [ClassData(typeof(LlcTopologyTestData))]
    public void Get_WhenRequestIsValid(LlcTopologyParameters data)
    {
        // arrange
        var frequency = data.Frequency;
        var temperature = data.Temperature;

        var hs = new HeatingSystem(new[]
            {
                new HeatingSystemData
                {
                    Key = frequency, Resistance = data.FrequencyData.resistance,
                    Inductance = data.FrequencyData.inductance
                }
            },
            new[]
            {
                new HeatingSystemData
                {
                    Key = temperature, Resistance = data.TemperatureData.resistance,
                    Inductance = data.TemperatureData.inductance
                }
            });


        var llc = new LlcTopology(hs);
        llc.Inductance = data.Inductance;
        llc.Capacitance = data.Capacitance;

        // act
        var actualResistance = llc.Resistance(frequency, temperature);
        var actualReactance = llc.Reactance(frequency, temperature);
        var actualImpedance = llc.Impedance(frequency, temperature);
        var actualParallelReactance = llc.ParallelReactance(frequency, temperature);


        AssertWithRelativeTolerance(data.ExpectedResistance, actualResistance, data.Tolerance);
        AssertWithRelativeTolerance(data.ExpectedReactance, actualReactance, data.Tolerance);
        AssertWithRelativeTolerance(data.ExpectedImpedance, actualImpedance, data.Tolerance);
        AssertWithRelativeTolerance(data.ExpectedParallelReactance, actualParallelReactance, data.Tolerance);
    }


    [Fact]
    public void Dispose()
    {
        // arrange

        var inputData = new List<HeatingSystemData>
        {
            new() {Key = 0, Resistance = 0, Inductance = 0}
        };
        var hs = new HeatingSystem(inputData, inputData);
        var llc = new LlcTopology(hs);
        llc.Dispose();


        // act
        void GetResistance()
        {
            llc.Resistance(0, 0);
        }

        void GetReactance()
        {
            llc.Reactance(0, 0);
        }

        void GetImpedance()
        {
            llc.Impedance(0, 0);
        }

        void GetParallelReactance()
        {
            llc.ParallelReactance(0, 0);
        }


        // assert
        Assert.ThrowsAny<ObjectDisposedException>(GetResistance);
        Assert.ThrowsAny<ObjectDisposedException>(GetReactance);
        Assert.ThrowsAny<ObjectDisposedException>(GetImpedance);
        Assert.ThrowsAny<ObjectDisposedException>(GetParallelReactance);
    }


    private static void AssertWithRelativeTolerance(double expected, double actual, double tolerance)
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (expected == actual)
            return;

        var aExpected = Math.Abs(expected);
        var aActual = Math.Abs(actual);


        var max = Math.Max(aExpected, aActual);
        var min = Math.Min(aExpected, aActual);

        var r = min / max;

        var isWithin = 1 - r <= tolerance;

        if (isWithin is false)
            throw new EqualException(expected, actual);
    }
}

public class LlcTopologyParameters
{
    public required double Frequency { get; init; }
    public required double Temperature { get; init; }

    public required (double resistance, double inductance) FrequencyData { get; init; }
    public required (double resistance, double inductance) TemperatureData { get; init; }

    public required double Inductance { get; init; }
    public required double Capacitance { get; init; }

    public required double ExpectedResistance { get; init; }
    public required double ExpectedReactance { get; init; }
    public required double ExpectedImpedance { get; init; }
    public required double ExpectedParallelReactance { get; init; }

    /// <summary>
    ///     A relative threshold between expected and actual values below which numbers will be considered the same
    /// </summary>
    public double Tolerance { get; init; } = 0.05;
}

public class LlcTopologyTestData : IEnumerable<object[]>
{
    private readonly List<object[]> _data = new()
    {
        new object[]
        {
            new LlcTopologyParameters
            {
                Frequency = 0,
                Temperature = 0,
                FrequencyData = (0, 0),
                TemperatureData = (0, 0),
                Inductance = 0,
                Capacitance = 0,
                ExpectedResistance = 0,
                ExpectedReactance = 0,
                ExpectedImpedance = 0,
                ExpectedParallelReactance = 0
            }
        },
        new object[]
        {
            new LlcTopologyParameters
            {
                Frequency = 5,
                Temperature = 0,
                FrequencyData = (20, 12),
                TemperatureData = (20, 12),
                Inductance = 22,
                Capacitance = 1,
                ExpectedResistance = 1.96e-8,
                ExpectedReactance = 691.11,
                ExpectedImpedance = 691.11,
                ExpectedParallelReactance = -0.0318
            }
        }
    };

    public IEnumerator<object[]> GetEnumerator()
    {
        return _data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
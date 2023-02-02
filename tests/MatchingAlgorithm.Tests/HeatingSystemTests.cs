using System.Collections;
using MatchingAlgorithm.Wrapper;

namespace MatchingAlgorithm.Tests;

public class HeatingSystemTests
{
    [Theory]
    [ClassData(typeof(HeatingSystemTestData))]
    public void Get_WhenRequestIsValid(HeatingSystemTestParameters data)
    {
        // arrange
        var frequencyData = data.Frequency;
        var temperatureData = data.Temperature;
        var frequency = frequencyData.Key;
        var temperature = temperatureData.Key;

        var hs = new HeatingSystem(new[] { frequencyData }, new[] { temperatureData });

        // act
        var actualResistance = hs.Resistance(frequency, temperature);
        var actualInductance = hs.Inductance(frequency, temperature);
        var actualImpedance = hs.Impedance(frequency, temperature);

        // assert

        Assert.Equal(data.ExpectedResistance, actualResistance);
        Assert.Equal(data.ExpectedInductance, actualInductance);
        Assert.Equal(data.ExpectedImpedance, actualImpedance);
    }

    [Fact]
    public void Get_WhenRequestIsInvalid()
    {
        // arrange
        var hs = new HeatingSystem(Enumerable.Empty<HeatingSystemData>(), Enumerable.Empty<HeatingSystemData>());


        // act
        void GetResistance()
        {
            hs.Resistance(0, 0);
        }

        void GetInductance()
        {
            hs.Inductance(0, 0);
        }

        void GetImpedance()
        {
            hs.Impedance(0, 0);
        }


        // assert
        Assert.ThrowsAny<ArgumentOutOfRangeException>(GetResistance);
        Assert.ThrowsAny<ArgumentOutOfRangeException>(GetInductance);
        Assert.ThrowsAny<ArgumentOutOfRangeException>(GetImpedance);
    }

    [Fact]
    public void Dispose()
    {
        // arrange
        var inputData = new List<HeatingSystemData>
        {
            new(0, 0, 0)
        };
        var hs = new HeatingSystem(inputData, inputData);
        hs.Dispose();


        // act
        void GetResistance()
        {
            hs.Resistance(0, 0);
        }

        void GetInductance()
        {
            hs.Inductance(0, 0);
        }

        void GetImpedance()
        {
            hs.Impedance(0, 0);
        }

        // assert
        Assert.ThrowsAny<ObjectDisposedException>(GetResistance);
        Assert.ThrowsAny<ObjectDisposedException>(GetInductance);
        Assert.ThrowsAny<ObjectDisposedException>(GetImpedance);
    }
}

public class HeatingSystemTestParameters
{
    public required HeatingSystemData Frequency { get; init; }
    public required HeatingSystemData Temperature { get; init; }

    public required double ExpectedResistance { get; init; }
    public required double ExpectedInductance { get; init; }
    public required double ExpectedImpedance { get; init; }
}

public class HeatingSystemTestData : IEnumerable<object[]>

{
    private readonly List<object[]> _data = new()
    {
        new object[]
        {
            new HeatingSystemTestParameters
            {
                Frequency = new HeatingSystemData(0, 0, 0),
                Temperature = new HeatingSystemData(0, 0, 0),
                ExpectedResistance = 0,
                ExpectedInductance = 0,
                ExpectedImpedance = 0
            }
        },
        new object[]
        {
            new HeatingSystemTestParameters
            {
                Frequency = new HeatingSystemData(10e3, 20e-3, 5e-6),
                Temperature = new HeatingSystemData(20, 1.13, 1.03),
                ExpectedResistance = 20e-3 * 1.13,
                ExpectedInductance = 5e-6 * 1.03,
                ExpectedImpedance = Math.Sqrt(20e-3 * 1.13 * (20e-3 * 1.13) + 5e-6 * 1.03 * (5e-6 * 1.03))
            }
        },
        new object[]
        {
            new HeatingSystemTestParameters
            {
                Frequency = new HeatingSystemData(52.32e3, 21.8e-3, 15.1e-5),
                Temperature = new HeatingSystemData(29.3, 1.09, 1.13),
                ExpectedResistance = 21.8e-3 * 1.09,
                ExpectedInductance = 15.1e-5 * 1.13,
                ExpectedImpedance = Math.Sqrt(21.8e-3 * 1.09 * (21.8e-3 * 1.09) + 15.1e-5 * 1.13 * (15.1e-5 * 1.13))
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
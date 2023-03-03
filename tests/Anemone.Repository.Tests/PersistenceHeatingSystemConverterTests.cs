namespace Anemone.Repository.Tests;

public class PersistenceHeatingSystemConverterTests
{
    [Fact]
    public void Serialize()
    {
        // arrange
        var expected = new PersistenceHeatingSystemModel
        {
            Name = "alabama",
            Id = "this should be ignored",
            FrequencyData = new HeatingSystemDataPointModel[]
            {
                new(0, 0, 0),
                new(20e3, 15, 12)
            },
            TemperatureData = new HeatingSystemDataPointModel[]
            {
                new(0, 0, 0),
                new(20, 1.1, 1.12),
                new(25, 2.31, 1.39),
                new(100, 22, 39)
            }
        };

        // act
        var expectedAsString = JsonSerializer.Serialize(expected);
        var actual = JsonSerializer.Deserialize<PersistenceHeatingSystemModel>(expectedAsString);


        // assert
        Assert.NotNull(actual);
        Assert.Null(actual.Id);
        Assert.Equal(expected.Name, actual.Name);
        foreach (var expectedData in expected.FrequencyData)
            Assert.Contains(actual.FrequencyData, actualData => actualData == expectedData);
        foreach (var expectedData in expected.TemperatureData)
            Assert.Contains(actual.TemperatureData, actualData => actualData == expectedData);
    }
}
namespace Anemone.Repository.Tests;

public class HeatingSystemRepositoryTestsTestContext
{
    public required string Directory { get; set; }

    /// <summary>
    ///     A filename without extension
    /// </summary>
    public required string FileName { get; init; }

    public string FilePath => Path.Combine(Directory, FileName + LocalRepositoryFileExtensions.HeatingSystem);
    public required PersistenceHeatingSystemModel TestData { get; init; }
    public string SerializedTestData => JsonSerializer.Serialize(TestData);
}
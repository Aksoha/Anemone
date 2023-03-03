using System.Collections;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace Anemone.Repository.Tests;

public class HeatingSystemRepositoryTests
{
    [Theory]
    [ClassData(typeof(CreateData))]
    public async Task Create(HeatingSystemRepositoryTestsTestContext testContext)
    {
        // arrange
        var workingDir = testContext.Directory;
        var fileSystemMock = new MockFileSystem();
        fileSystemMock.Directory.CreateDirectory(testContext.Directory);
        var loggerMock = Mock.Of<ILogger<HeatingSystemLocalRepository>>();
        var repositoryOptions = new LocalRepositoryOptions { WorkingDir = workingDir };

        var repository = new HeatingSystemLocalRepository(fileSystemMock.File, fileSystemMock.Directory, loggerMock,
            repositoryOptions);

        // act
        await repository.Create(testContext.TestData, testContext.FileName);

        // assert
        Assert.NotNull(testContext.TestData.Id);
        Assert.Equal(testContext.FileName, testContext.TestData.Id);
        Assert.True(fileSystemMock.File.Exists(testContext.FilePath));
        var fileContent = await fileSystemMock.File.ReadAllTextAsync(testContext.FilePath);
        Assert.Equal(testContext.SerializedTestData, fileContent);
    }

    [Fact]
    public async Task GetAllNames()
    {
        // arrange
        const string workingDir = @"c:\tests";

        var files = new Dictionary<string, MockFileData>
        {
            { @"E:\text.txt", new MockFileData(string.Empty) },
            { workingDir + @"\image.img", new MockFileData(string.Empty) },
            {
                workingDir + @"\validFile1" + LocalRepositoryFileExtensions.HeatingSystem,
                new MockFileData(string.Empty)
            },
            {
                workingDir + @"\validFile2" + LocalRepositoryFileExtensions.HeatingSystem,
                new MockFileData(string.Empty)
            }
        };

        var fileSystemMock = new MockFileSystem(files);
        var loggerMock = Mock.Of<ILogger<HeatingSystemLocalRepository>>();
        var repositoryOptions = new LocalRepositoryOptions { WorkingDir = workingDir };

        var repository = new HeatingSystemLocalRepository(fileSystemMock.File, fileSystemMock.Directory, loggerMock,
            repositoryOptions);


        // act
        var actualFileNames = (await repository.GetAllNames()).ToList();


        // assert
        var actualFilesWithCorrectExtension = files
            .Where(x => x.Key.EndsWith(LocalRepositoryFileExtensions.HeatingSystem)).Select(x => x.Key).ToList();
        Assert.Equal(actualFilesWithCorrectExtension.Count, actualFileNames.Count);

        foreach (var actualFile in actualFileNames)
            Assert.Contains(actualFilesWithCorrectExtension,
                fileNameWithExtension => Path.GetFileNameWithoutExtension(fileNameWithExtension) == actualFile);
    }

    [Theory]
    [ClassData(typeof(GetData))]
    public async Task Get(HeatingSystemRepositoryTestsTestContext testContext)
    {
        // arrange
        var workingDir = testContext.Directory;
        var fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { testContext.FilePath, new MockFileData(testContext.SerializedTestData) }
        });


        var loggerMock = Mock.Of<ILogger<HeatingSystemLocalRepository>>();
        var repositoryOptions = new LocalRepositoryOptions { WorkingDir = workingDir };

        var repository = new HeatingSystemLocalRepository(fileSystemMock.File, fileSystemMock.Directory, loggerMock,
            repositoryOptions);


        // act
        var actual = await repository.Get(testContext.FileName);


        // assert
        Assert.NotNull(actual);
        AssertHeatingSystemProperties(testContext, actual);
    }

    [Theory]
    [ClassData(typeof(GetAllData))]
    public async Task GetAll(List<HeatingSystemRepositoryTestsTestContext> testContext)
    {
        // arrange
        const string workingDir = GetAllData.Directory;
        var fileSystemMock = new MockFileSystem();
        foreach (var testContextEntry in testContext)
            fileSystemMock.AddFile(testContextEntry.FilePath, testContextEntry.SerializedTestData);


        var loggerMock = Mock.Of<ILogger<HeatingSystemLocalRepository>>();
        var repositoryOptions = new LocalRepositoryOptions { WorkingDir = workingDir };

        var repository = new HeatingSystemLocalRepository(fileSystemMock.File, fileSystemMock.Directory, loggerMock,
            repositoryOptions);

        // act
        var actualCollection = (await repository.GetAll()).ToList();

        // assert
        Assert.Equal(testContext.Count, actualCollection.Count);
        foreach (var testContextEntry in testContext)
        {
            var actualObj = actualCollection.Single(x => x.Id == testContextEntry.FileName);
            AssertHeatingSystemProperties(testContextEntry, actualObj);
        }
    }

    [Theory]
    [ClassData(typeof(UpdateData))]
    public async Task Update(HeatingSystemRepositoryTestsTestContext testContext)
    {
        // arrange
        var workingDir = testContext.Directory;
        var fileSystemMock = new MockFileSystem();
        fileSystemMock.Directory.CreateDirectory(testContext.Directory);
        fileSystemMock.AddFile(testContext.FilePath, testContext.SerializedTestData);
        testContext.TestData.Id = testContext.FilePath;


        var loggerMock = Mock.Of<ILogger<HeatingSystemLocalRepository>>();
        var repositoryOptions = new LocalRepositoryOptions { WorkingDir = workingDir };

        var repository = new HeatingSystemLocalRepository(fileSystemMock.File, fileSystemMock.Directory, loggerMock,
            repositoryOptions);


        // act
        testContext.TestData.Name = "new name";
        testContext.TestData.FrequencyData = new HeatingSystemDataPointModel[]
        {
            new(12.3e2, 0.3, 1.1),
            new(15e2, 0.11, 0.8)
        };
        testContext.TestData.TemperatureData = new HeatingSystemDataPointModel[]
        {
            new(32, 1.12, 5.12),
            new(547, 4.44, 1.987)
        };
        await repository.Update(testContext.FileName, testContext.TestData);
        var file = await fileSystemMock.File.ReadAllTextAsync(testContext.FilePath);
        var actual = JsonSerializer.Deserialize<PersistenceHeatingSystemModel>(file);


        // assert
        Assert.NotNull(actual);
        actual.Id = testContext.FileName;
        AssertHeatingSystemProperties(testContext, actual);
    }

    [Theory]
    [ClassData(typeof(DeleteData))]
    public async Task Delete(List<HeatingSystemRepositoryTestsTestContext> testContext)
    {
        // arrange
        const string workingDir = DeleteData.Directory;
        var fileSystemMock = new MockFileSystem();
        foreach (var testContextEntry in testContext)
        {
            fileSystemMock.AddFile(testContextEntry.FilePath, testContextEntry.SerializedTestData);
            testContextEntry.TestData.Id = testContextEntry.FilePath;
        }


        var loggerMock = Mock.Of<ILogger<HeatingSystemLocalRepository>>();
        var repositoryOptions = new LocalRepositoryOptions { WorkingDir = workingDir };

        var repository = new HeatingSystemLocalRepository(fileSystemMock.File, fileSystemMock.Directory, loggerMock,
            repositoryOptions);

        // act
        var itemToDelete = testContext.First();
        await repository.Delete(itemToDelete.TestData);

        // assert
        Assert.False(fileSystemMock.File.Exists(itemToDelete.FilePath));
        Assert.Null(itemToDelete.TestData.Id);
    }

    private static void AssertHeatingSystemProperties(HeatingSystemRepositoryTestsTestContext expected,
        PersistenceHeatingSystemModel actual)
    {
        Assert.Equal(expected.FileName, actual.Id);
        Assert.Equal(expected.TestData.Name, actual.Name);
        Assert.Equal(expected.TestData.FrequencyData.Count(), actual.FrequencyData.Count());
        Assert.Equal(expected.TestData.TemperatureData.Count(), actual.TemperatureData.Count());

        foreach (var expectedData in expected.TestData.FrequencyData)
            Assert.Contains(actual.FrequencyData, actualData => actualData == expectedData);

        foreach (var expectedData in expected.TestData.TemperatureData)
            Assert.Contains(actual.TemperatureData, actualData => actualData == expectedData);
    }
}

#region TestData

file class CreateData : IEnumerable<object[]>
{
    private readonly List<object[]> _data = new()
    {
        new object[]
        {
            new HeatingSystemRepositoryTestsTestContext
            {
                Directory = @"c:\hs",
                FileName = "file_under_test",
                TestData = new PersistenceHeatingSystemModel
                {
                    Name = "test data",
                    FrequencyData = new HeatingSystemDataPointModel[]
                    {
                        new(0, 0, 0),
                        new(10e3, 0.023, 2e-8),
                        new(33.3e3, 0.025, 13e-8),
                        new(12e3, 0.13, 2.2e-7)
                    },
                    TemperatureData = new HeatingSystemDataPointModel[]
                    {
                        new(0, 0, 0),
                        new(20, 1, 1),
                        new(501.1, 1.15, 1.32)
                    }
                }
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

file class GetData : IEnumerable<object[]>
{
    private readonly List<object[]> _data = new()
    {
        new object[]
        {
            new HeatingSystemRepositoryTestsTestContext
            {
                Directory = @"c:\hs",
                FileName = "file_under_test",
                TestData = new PersistenceHeatingSystemModel
                {
                    Name = "test data",
                    FrequencyData = new HeatingSystemDataPointModel[]
                    {
                        new(0, 0, 0),
                        new(10e3, 0.023, 2e-8),
                        new(33.3e3, 0.025, 13e-8),
                        new(12e3, 0.13, 2.2e-7)
                    },
                    TemperatureData = new HeatingSystemDataPointModel[]
                    {
                        new(0, 0, 0),
                        new(20, 1, 1),
                        new(501.1, 1.15, 1.32)
                    }
                }
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

file class GetAllData : IEnumerable<object[]>
{
    public const string Directory = @"c:\hs";

    private readonly List<object[]> _data = new()
    {
        new object[]
        {
            new List<HeatingSystemRepositoryTestsTestContext>
            {
                new()
                {
                    Directory = Directory,
                    FileName = "file_under_test",
                    TestData = new PersistenceHeatingSystemModel
                    {
                        Name = "test data",
                        FrequencyData = new HeatingSystemDataPointModel[]
                        {
                            new(0, 0, 0),
                            new(10e3, 0.023, 2e-8),
                            new(33.3e3, 0.025, 13e-8),
                            new(12e3, 0.13, 2.2e-7)
                        },
                        TemperatureData = new HeatingSystemDataPointModel[]
                        {
                            new(0, 0, 0),
                            new(20, 1, 1),
                            new(501.1, 1.15, 1.32)
                        }
                    }
                },
                new()
                {
                    Directory = Directory,
                    FileName = "file_under_test2",
                    TestData = new PersistenceHeatingSystemModel
                    {
                        Name = "test data2",
                        FrequencyData = new HeatingSystemDataPointModel[]
                        {
                            new(13e4, 0.011, 21e-6)
                        },
                        TemperatureData = new HeatingSystemDataPointModel[]
                        {
                            new(37.6, 0.87, 2.38)
                        }
                    }
                }
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

file class UpdateData : IEnumerable<object[]>
{
    private readonly List<object[]> _data = new()
    {
        new object[]
        {
            new HeatingSystemRepositoryTestsTestContext
            {
                Directory = @"c:\hs",
                FileName = "file_under_test",
                TestData = new PersistenceHeatingSystemModel
                {
                    Name = "test data",
                    FrequencyData = new HeatingSystemDataPointModel[]
                    {
                        new(0, 0, 0),
                        new(10e3, 0.023, 2e-8),
                        new(33.3e3, 0.025, 13e-8),
                        new(12e3, 0.13, 2.2e-7)
                    },
                    TemperatureData = new HeatingSystemDataPointModel[]
                    {
                        new(0, 0, 0),
                        new(20, 1, 1),
                        new(501.1, 1.15, 1.32)
                    }
                }
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

file class DeleteData : IEnumerable<object[]>
{
    public const string Directory = @"c:\hs";

    private readonly List<object[]> _data = new()
    {
        new object[]
        {
            new List<HeatingSystemRepositoryTestsTestContext>
            {
                new()
                {
                    Directory = Directory,
                    FileName = "file_under_test",
                    TestData = new PersistenceHeatingSystemModel
                    {
                        Name = "test data",
                        FrequencyData = new HeatingSystemDataPointModel[]
                        {
                            new(0, 0, 0),
                            new(10e3, 0.023, 2e-8),
                            new(33.3e3, 0.025, 13e-8),
                            new(12e3, 0.13, 2.2e-7)
                        },
                        TemperatureData = new HeatingSystemDataPointModel[]
                        {
                            new(0, 0, 0),
                            new(20, 1, 1),
                            new(501.1, 1.15, 1.32)
                        }
                    }
                },
                new()
                {
                    Directory = Directory,
                    FileName = "file_under_test2",
                    TestData = new PersistenceHeatingSystemModel
                    {
                        Name = "test data2",
                        FrequencyData = new HeatingSystemDataPointModel[]
                        {
                            new(13e4, 0.011, 21e-6)
                        },
                        TemperatureData = new HeatingSystemDataPointModel[]
                        {
                            new(37.6, 0.87, 2.38)
                        }
                    }
                }
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

#endregion
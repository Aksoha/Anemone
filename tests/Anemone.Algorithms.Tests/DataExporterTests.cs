using System.Data;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Anemone.Algorithms.Report;
using Bogus;
using Moq;

namespace Anemone.Algorithms.Tests;

public class DataExporterTests
{
    private readonly Faker<ProductModel> _productFaker;

    public DataExporterTests()
    {
        _productFaker = new Faker<ProductModel>()
            .RuleFor(x => x.Id, f => f.Random.Int(0).ToString())
            .RuleFor(x => x.ProductName, f => f.Commerce.ProductName());
    }

    [Theory]
    [InlineData(".xlsx")]
    [InlineData(".xls")]
    [InlineData("")]
    public async Task Export_WhenExtensionIsNotSupported(string extension)
    {
        // arrange
        var fileMock = new Mock<IFile>();
        var file = fileMock.Object;
        var filePath = Path.Join(@"C:/test", extension);
        var dataExporter = new DataExporter(file);


        // act
        Task Act()
        {
            return dataExporter.ExportToCsv(filePath, new DataTable());
        }

        // assert
        await Assert.ThrowsAsync<NotSupportedException>(Act);
    }

    [Fact]
    public async Task Export_WhenDataIsValid()
    {
        // arrange
        const string directory = @"C:/test";
        var filePath = Path.Join(directory, "fileUnderTest.csv");
        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddDirectory(directory);
        var file = mockFileSystem.File;

        var products = _productFaker.Generate(5);

        var table = new DataTable();
        AppendHeaderRow(table);
        AppendContentRows(table, products);

        var dataExporter = new DataExporter(file);

        // act
        await dataExporter.ExportToCsv(filePath, table);


        // assert
        var actualLines = file.ReadLines(filePath).ToList();
        Assert.Equal(products.Count + 1, actualLines.Count);

        var idx = 0;
        var rows = actualLines.Select(line => line.Split(",")).ToArray();
        
        var firstRow = rows.First();
        Assert.Equal(2, firstRow.Length);
        Assert.Equal("Id", firstRow[0]);
        Assert.Equal("ProductName", firstRow[1]);

        foreach (var columns in rows.Skip(1))
        {
            Assert.Equal(2, columns.Length);

            var product = products[idx];
            Assert.Equal(product.Id, columns[0].Trim('"'));
            Assert.Equal(product.ProductName, columns[1].Trim('"'));
            idx++;
        }
    }

    private static void AppendContentRows(DataTable table, List<ProductModel> content)
    {
        foreach (var contentItem in content)
        {
            var row = table.NewRow();
            row["Id"] = contentItem.Id;
            row["ProductName"] = contentItem.ProductName;
            table.Rows.Add(row);
        }
    }

    private static void AppendHeaderRow(DataTable table)
    {
        table.Columns.Add("Id");
        table.Columns.Add("ProductName");
    }

    private class ProductModel
    {
        public string Id { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
    }
}
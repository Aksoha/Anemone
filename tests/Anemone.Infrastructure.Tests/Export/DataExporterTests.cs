using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Anemone.Core.Export;
using Anemone.Infrastructure.Export;
using Bogus;

namespace Anemone.Infrastructure.Tests.Export;

public class DataExporterTests
{
    private Faker<ProductModel> _productFaker;


    public DataExporterTests()
    {
        InitializeFaker();
    }

    private DataExporter? DataExporter { get; set; }

    private MockFileSystem MockFileSystem { get; } = new();
    private IFile File => MockFileSystem.File;


    [Fact]
    public async Task ExportToCsv_WhenDataIsValid()
    {
        // arrange
        var filePath = CreateFile("fileUnderTest.csv");
        var expectedTable = CreateTable();
        CreateDataExporter();


        // act
        await DataExporter.ExportToCsv(filePath, expectedTable);


        // assert
        var actualTable = CsvToTable(filePath);
        VerifyTableContentIsSame(expectedTable, actualTable);
    }


    [Theory]
    [InlineData(".xlsx")]
    [InlineData(".xls")]
    [InlineData("")]
    public async Task ExportToCsv_ThrowsExportFileTypeNotSupportedException(string extension)
    {
        // arrange
        var filePath = CreateFile($"fileUnderTest{extension}");
        CreateDataExporter();


        // act
        Task Act()
        {
            return DataExporter.ExportToCsv(filePath, new DataTable());
        }


        // assert
        var exception = await Assert.ThrowsAsync<ExportFileTypeNotSupportedException>(Act);
        Assert.Equal(extension, exception.NotSupportedExtension);
        Assert.Equal(filePath, exception.FilePath);
    }


    [MemberNotNull(nameof(_productFaker))]
    private void InitializeFaker()
    {
        _productFaker = new Faker<ProductModel>()
            .RuleFor(x => x.Id, f => f.Random.Int(0).ToString())
            .RuleFor(x => x.ProductName, f => f.Commerce.ProductName());
    }

    [MemberNotNull(nameof(DataExporter))]
    private void CreateDataExporter()
    {
        DataExporter = new DataExporter(File);
    }


    private string CreateFile(string fileName)
    {
        const string directory = @"C:/test";
        MockFileSystem.AddDirectory(directory);
        var filePath = Path.Join(directory, fileName);
        return filePath;
    }

    private DataTable CreateTable()
    {
        var table = new DataTable();
        var products = _productFaker.Generate(5);

        table.Columns.Add();
        table.Columns.Add();

        foreach (var contentItem in products)
        {
            var row = table.NewRow();
            row[0] = contentItem.Id;
            row[1] = contentItem.ProductName;
            table.Rows.Add(row);
        }

        return table;
    }

    private DataTable CsvToTable(string filePath)
    {
        var lines = GetFileLines(filePath);
        return LinesToTable(lines);
    }

    private List<string> GetFileLines(string filePath)
    {
        return File.ReadLines(filePath).ToList();
    }

    private static DataTable LinesToTable(IReadOnlyCollection<string> rows)
    {
        var table = new DataTable();

        var colCount = (rows.MaxBy(line => line.Split(",").Length) ?? string.Empty)
            .Count(c => c == ',') + 1;

        CreateColumns(table, colCount);

        foreach (var row in rows) WriteTableRow(table, row.Split(','));

        return table;
    }

    private static void CreateColumns(DataTable table, int count)
    {
        for (var i = 0; i < count; i++) table.Columns.Add();
    }

    private static void WriteTableRow(DataTable table, IEnumerable<string> row)
    {
        var rowTable = table.NewRow();
        var colIdx = 0;
        foreach (var col in row)
        {
            rowTable[colIdx] = col.Trim('"');
            colIdx++;
        }

        table.Rows.Add(rowTable);
    }


    private static void VerifyTableContentIsSame(DataTable expected, DataTable actual)
    {
        VerifyTableSizeIsSame(expected, actual);

        for (var i = 0; i < expected.Rows.Count; i++)
        {
            var expectedRow = expected.Rows[i];
            var actualRow = actual.Rows[i];

            for (var index = 0; index < actualRow.ItemArray.Length; index++)
            {
                var expectedItem = expectedRow.ItemArray[index];
                var actualItem = actualRow.ItemArray[index];

                Assert.Equal(expectedItem, actualItem);
            }
        }
    }

    private static void VerifyTableSizeIsSame(DataTable expected, DataTable actual)
    {
        Assert.Equal(expected.Rows.Count, actual.Rows.Count);
        Assert.Equal(expected.Columns.Count, actual.Columns.Count);
    }

    private class ProductModel
    {
        public string Id { get; } = string.Empty;
        public string ProductName { get; } = string.Empty;
    }
}
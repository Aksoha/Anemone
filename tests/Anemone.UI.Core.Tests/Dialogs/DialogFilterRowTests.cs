using Anemone.Core;
using Anemone.Core.Common.ValueObjects;
using Anemone.UI.Core.Dialogs;

namespace Anemone.UI.Core.Tests.Dialogs;

public class DialogFilterRowTests
{
    [Fact]
    public void ToString_WhenRowHasSingleExtension()
    {
        // arrange
        var filterRow = new DialogFilterRow("Csv file", new[] { FileExtension.Csv });

        // act
        string actualString = filterRow;


        // assert
        Assert.Equal("Csv file|*.csv", actualString);
    }

    [Fact]
    public void ToString_WhenRowHasMultipleExtension()
    {
        // arrange
        var filterRow =
            new DialogFilterRow("Sheet files", new[] { FileExtension.Csv, FileExtension.Xls });


        // act
        string actualString = filterRow;


        // assert
        Assert.Equal("Sheet files|*.csv;*.xls", actualString);
    }

    [Fact]
    public void ToString_WhenRowHasNoExtensions_ThrowsDialogExtensionCountException()
    {
        // arrange
        var filterRow = new DialogFilterRow("Not relevant to this test", Array.Empty<Func<FileExtension>>());

        // act
        void Act()
        {
            // ReSharper disable once UnusedVariable, it is to invoke conversion
            string actualString = filterRow;
        }

        // assert
        Assert.Throws<DialogExtensionCountException>(Act);
    }
}
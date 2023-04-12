using Anemone.Core.Dialogs;

namespace Anemone.Core.Tests.Dialogs;

public class DialogFilterRowTests
{
    [Fact]
    public void ToString_WhenRowHasSingleExtension()
    {
        // arrange
        var filterRow = new DialogFilterRow("Csv file", new[] { DialogFilterExtension.Csv });

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
            new DialogFilterRow("Sheet files", new[] { DialogFilterExtension.Csv, DialogFilterExtension.Xls });


        // act
        string actualString = filterRow;


        // assert
        Assert.Equal("Sheet files|*.csv;*.xls", actualString);
    }

    [Fact]
    public void ToString_WhenRowHasNoExtensions_ThrowsDialogExtensionCountException()
    {
        // arrange
        var filterRow = new DialogFilterRow("Not relevant to this test", Array.Empty<Func<DialogFilterExtension>>());

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
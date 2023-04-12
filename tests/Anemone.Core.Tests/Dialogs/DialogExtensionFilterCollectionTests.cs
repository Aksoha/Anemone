using Anemone.Core.Dialogs;

namespace Anemone.Core.Tests.Dialogs;

public class DialogFilterCollectionTests
{
    [Fact]
    public void ToString_WhenCollectionHasSingleRow()
    {
        // arrange
        var collection = new DialogFilterCollection();
        var row = new DialogFilterRow("Csv files", new[] { DialogFilterExtension.Csv });
        collection.AddFilterRow(row);
        
        
        // act
        string actualString = collection;
        
        
        // assert
        Assert.Equal(row, actualString);
    }

    [Fact]
    public void ToString_WhenCollectionHasMultipleRows()
    {
        // arrange
        var collection = new DialogFilterCollection();
        collection.AddFilterRow( new DialogFilterRow("Csv files", new[] { DialogFilterExtension.Csv }));
        collection.AddFilterRow( new DialogFilterRow("All files", new[] { DialogFilterExtension.All }));
        
        
        // act
        string actualString = collection;
        
        
        // assert
        Assert.Equal("Csv files|*.csv|All files|*.*", actualString);  
    }

    [Fact]
    public void ToString_WhenCollectionHasNoRows_ThrowsDialogFilterCountException()
    {
        // arrange
        var collection = new DialogFilterCollection();

        // act
        void Act()
        {
            // ReSharper disable once UnusedVariable, it is to invoke conversion
            string actualString = collection;
        }

        // assert
        Assert.Throws<DialogFilterCountException>(Act);
    }
}
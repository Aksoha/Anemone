using Anemone.Core;
using Anemone.Core.Common.ValueObjects;
using Anemone.UI.Core.Dialogs;

namespace Anemone.UI.Core.Tests.Dialogs;

public class DialogFilterCollectionTests
{
    [Fact]
    public void ToString_WhenCollectionHasSingleRow()
    {
        // arrange
        var collection = new DialogFilterCollection();
        var row = new DialogFilterRow("Csv files", new[] { FileExtension.Csv });
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
        collection.AddFilterRow( new DialogFilterRow("Csv files", new[] { FileExtension.Csv }));
        collection.AddFilterRow( new DialogFilterRow("All files", new[] { FileExtension.All }));
        
        
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
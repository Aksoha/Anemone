namespace MatchingAlgorithm.Tests;

public class TransformerCalculatorTests
{
    [Theory]
    [InlineData(50.3, 1, 50.3)]
    [InlineData(23.0, 2, 11.5)]
    [InlineData(92.16, 18, 5.12)]
    public void CurrentToPrimarySide_WhenDataIsValid(double current, double turnRatio, double result)
    {
        // act
        var act = TransformerCalculator.CurrentToPrimarySide(current, turnRatio);
        
        // assert
        Assert.Equal(result, act);
    }

    [Fact]
    public void CurrentToPrimarySide_ThrowsOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>TransformerCalculator.CurrentToPrimarySide(15, 0));
    }
    
    [Theory]
    [InlineData(11.9, 1, 11.9)]
    [InlineData(6.13, 3, 18.39)]
    [InlineData(4.22, 7, 29.54)]
    public void CurrentToSecondarySide_WhenDataIsValid(double current, double turnRatio, double result)
    {
        // act
        var act = TransformerCalculator.CurrentToSecondarySide(current, turnRatio);
        
        // assert
        Assert.Equal(result, act);
    }
    
    [Fact]
    public void CurrentToSecondarySide_ThrowsOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>TransformerCalculator.CurrentToSecondarySide(0.12, 0));
    }


    [Theory]
    [InlineData(23.0, 1, 23.0)]
    [InlineData(110.1, 2, 220.2)]
    [InlineData(230.0, 4, 920.0)]
    public void VoltageToPrimarySide_WhenDataIsValid(double voltage, double turnRatio, double result)
    {
        // act
        var act = TransformerCalculator.VoltageToPrimarySide(voltage, turnRatio);
        
        // assert
        Assert.Equal(result, act);
    }




    [Theory]
    [InlineData(156.0, 1, 156.0)]
    [InlineData(66.6, 2, 33.3)]
    [InlineData(5.0, 5, 1.0)]
    public void VoltageToSecondarySide_WhenDataIsValid(double voltage, double turnRatio, double result)
    {
        // act
        var act = TransformerCalculator.VoltageToSecondarySide(voltage, turnRatio);
        
        // assert
        Assert.Equal(result, act);
    }
    
    
    
    [Fact]
    public void VoltageToSecondarySide_ThrowsOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>TransformerCalculator.VoltageToSecondarySide(230, 0));
    }


    [Theory]
    [InlineData(203.3, 1, 203.3)]
    [InlineData(15.12, 2, 3.78)]
    [InlineData(55.24, 10, 0.5524)]
    public void ResistanceToPrimary_WhenDataIsValid(double resistance, double turnRatio, double result)
    {
        // act
        var act = TransformerCalculator.ResistanceToPrimary(resistance, turnRatio);
        
        // assert
        Assert.Equal(result, act);
    }

    [Fact]
    public void ResistanceToPrimary_ThrowsOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>TransformerCalculator.ResistanceToPrimary(230, 0));
    }
    
    
    [Theory]
    [InlineData(11.8, 1, 11.8)]
    [InlineData(25.5, 2, 102.0)]
    [InlineData(0.98, 6, 35.28)]
    public void ResistanceToSecondary_WhenDataIsValid(double resistance, double turnRatio, double result)
    {
        // act
        var act = TransformerCalculator.ResistanceToSecondary(resistance, turnRatio);
        
        // assert
        Assert.Equal(result, act);
    }
}
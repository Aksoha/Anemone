namespace MatchingAlgorithm;

public static class TransformerCalculator
{
    public static double CurrentToPrimarySide(double current, double turnRatio)
    {
        if (turnRatio == 0)
            throw new ArgumentOutOfRangeException(nameof(turnRatio));
        return current / turnRatio;
    }

    public static double CurrentToSecondarySide(double current, double turnRatio)
    {
        if (turnRatio == 0)
            throw new ArgumentOutOfRangeException(nameof(turnRatio));
        return current * turnRatio;
    }
    
    public static double VoltageToPrimarySide(double voltage, double turnRatio)
    {
        return voltage * turnRatio;
    }
    
    public static double VoltageToSecondarySide(double voltage, double turnRatio)
    {
        if (turnRatio == 0)
            throw new ArgumentOutOfRangeException(nameof(turnRatio));
        return voltage / turnRatio;
    }


    public static double ResistanceToPrimary(double resistance, double turnRatio)
    {
        if (turnRatio == 0)
            throw new ArgumentOutOfRangeException(nameof(turnRatio));
        return resistance / (turnRatio * turnRatio);
    }
    
    public static double ResistanceToSecondary(double resistance, double turnRatio)
    {
        return resistance * turnRatio * turnRatio;
    }


}
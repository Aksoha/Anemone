namespace MatchingAlgorithm;

public class MatchingParameter : ICloneable
{
    public required IEnumerable<double> Frequency {get; set; }
    public required IEnumerable<double> Temperature {get; set; }
    public required IEnumerable<double> TurnRatio {get; set; }
    public required double VoltageLimit {get; set; }
    public required double CurrentLimit {get; set; }
    public required double ExpectedPower {get; set; }
    
    public virtual object Clone()
    {
        return MemberwiseClone();
    }
}
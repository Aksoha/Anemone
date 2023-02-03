using System.Numerics;

namespace MatchingAlgorithm;

public class MatchingResult
{
    public required double Resistance { get; set; }
    public required double Reactance { get; set; }
    public required Complex Impedance { get; set; }
    public required double Frequency { get; set; }
    public required double Voltage { get; set; }
    public required double Current { get; set; }
    public required double Power { get; set; }
    public required double TurnRatio { get; set; }
}
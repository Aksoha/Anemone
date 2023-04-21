using System.Numerics;

namespace Anemone.Core.EnergyMatching.Results;

public class MatchingResultPoint
{
    public double Resistance { get; set; }
    public double Reactance { get; set; }
    public double Impedance => Complex.Abs(new Complex(Resistance, Reactance));
    public double Frequency { get; set; }
    public double Temperature { get; set; }
    public double Voltage { get; set; }
    public double Current { get; set; }
    public double Power { get; set; }
    public double PhaseShift => new Complex(Resistance, Reactance).Phase;
}
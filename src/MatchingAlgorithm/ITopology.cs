using System.Numerics;

namespace MatchingAlgorithm;

public interface ITopology
{
    public double Resistance(double frequency, double temperature);
    public double Reactance(double frequency, double temperature);
    public Complex Impedance(double frequency, double temperature);
    
}
namespace MatchingAlgorithm.Llc;

public interface ILlcTopology : ITopology
{
    double Inductance { get; set; }
    double Capacitance { get; set; }
    double ParallelReactance(double frequency, double temperature);
}
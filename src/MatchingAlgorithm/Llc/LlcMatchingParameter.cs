namespace MatchingAlgorithm.Llc;

public class LlcMatchingParameter : MatchingParameter
{
    public required IEnumerable<double> Inductance {get; set; }
    public required IEnumerable<double> Capacitance {get; set; }
}
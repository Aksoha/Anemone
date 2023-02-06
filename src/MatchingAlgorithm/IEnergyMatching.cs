namespace MatchingAlgorithm;

public interface IEnergyMatching<TResult> where TResult : MatchingResult
{
    IEnumerable<TResult> EnergyMatching();
}
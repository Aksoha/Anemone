namespace MatchingAlgorithm;

public interface IEnergyMatching<TParameter, TResult>
    where TParameter : MatchingParameter where TResult : MatchingResult
{
    IEnumerable<TResult> EnergyMatching(TParameter parameters);
}
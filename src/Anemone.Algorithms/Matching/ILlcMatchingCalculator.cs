using MatchingAlgorithm.Llc;
using LlcMatchingParameter = Anemone.Algorithms.Models.LlcMatchingParameter;
using LlcMatchingResult = Anemone.Algorithms.Models.LlcMatchingResult;

namespace Anemone.Algorithms.Matching;


/// <summary>
/// Provides a way to perform calculation of the <see cref="LlcMatching"/> algorithm.
/// </summary>
public interface ILlcMatchingCalculator : IMatchingCalculator<LlcMatchingParameter, LlcMatchingResult>
{
    
}
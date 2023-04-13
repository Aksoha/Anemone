using Anemone.Algorithms.Models;
using MatchingAlgorithm.Llc;
using LlcMatchingResult = Anemone.Algorithms.Models.LlcMatchingResult;

namespace Anemone.Algorithms.Matching;


/// <summary>
/// Provides a way to perform calculation of the <see cref="LlcMatching"/> algorithm.
/// </summary>
public interface ILlcMatchingCalculator : IMatchingCalculator<LlcMatchingParameters, LlcMatchingResult>
{
    
}
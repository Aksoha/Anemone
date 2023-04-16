using Anemone.Algorithms.Models;
using MatchingAlgorithm.Llc;

namespace Anemone.Algorithms.Matching;

/// <summary>
///     Provides a way to perform calculation of the <see cref="LlcMatching" /> algorithm.
/// </summary>
public interface ILlcMatchingCalculator : IMatchingCalculator<LlcMatchingParameters, LlcMatchingResultSummary>
{
}
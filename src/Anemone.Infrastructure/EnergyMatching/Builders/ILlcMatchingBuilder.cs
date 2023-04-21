using Anemone.Core.EnergyMatching.Builders;
using MatchingAlgorithm.Llc;

namespace Anemone.Infrastructure.EnergyMatching.Builders;

/// <summary>
///     Builds <see cref="LlcMatching" /> algorithm object.
/// </summary>
internal interface ILlcMatchingBuilder : IMatchingBuilder<LlcMatching, LlcMatchingBuildArgs>
{
}
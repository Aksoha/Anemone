using Anemone.Core.EnergyMatching.Builders;
using MatchingAlgorithm;

namespace Anemone.Infrastructure.EnergyMatching.Builders;

/// <summary>
///     Builds <see cref="MatchingBase" /> algorithm object.
/// </summary>
/// <typeparam name="TAlgorithm">The type of matching algorithm.</typeparam>
/// <typeparam name="TArgs">The type of parameter.</typeparam>
internal interface IMatchingBuilder<out TAlgorithm, in TArgs>
    where TAlgorithm : MatchingBase where TArgs : MatchingBuildArgsBase
{
    /// <summary>
    ///     Creates the instance of <typeparamref name="TAlgorithm" />.
    /// </summary>
    /// <param name="parameters">The building parameters.</param>
    TAlgorithm Build(TArgs parameters);
}
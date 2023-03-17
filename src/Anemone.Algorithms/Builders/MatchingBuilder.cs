using MatchingAlgorithm;

namespace Anemone.Algorithms.Builders;


/// <summary>
/// Builds <see cref="MatchingBase"/> algorithm object.
/// </summary>
/// <typeparam name="TAlgorithm">The type of matching algorithm.</typeparam>
/// <typeparam name="TArgs">The type of parameter.</typeparam>
public interface IMatchingBuilder<out TAlgorithm, in TArgs> where TAlgorithm : MatchingBase where TArgs : MatchingBuildArgsBase
{
    /// <summary>
    /// Creates the instance of <typeparamref name="TAlgorithm"/>.
    /// </summary>
    /// <param name="parameters">The building parameters.</param>
    TAlgorithm Build(TArgs parameters);
}
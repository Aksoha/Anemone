using Anemone.Algorithms.Models;

namespace Anemone.Algorithms.Builders;

/// <summary>
/// The parameters used in the <see cref="IMatchingBuilder{TAlgorithm,TParameters}"/> building process.
/// </summary>
/// <typeparam name="T">The type of parameters.</typeparam>
public class MatchingBuildArgs<T> : MatchingBuildArgsBase where T : MatchingParametersBase
{
    public required T Parameter { get; set; }
}
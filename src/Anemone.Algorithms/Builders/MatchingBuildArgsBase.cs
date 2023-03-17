using Anemone.Algorithms.Models;

namespace Anemone.Algorithms.Builders;

/// <summary>
/// The parameters usd in the <see cref="IMatchingBuilder{TAlgorithm,TParameters}"/> building process.
/// </summary>
public class MatchingBuildArgsBase
{
    public required HeatingSystem HeatingSystem { get; set; }
}
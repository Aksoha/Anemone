using System.Threading.Tasks;
using Anemone.Algorithms.Models;
using Anemone.Repository.HeatingSystemData;

namespace Anemone.Algorithms.Matching;

/// <summary>
///     Provides a way to perform calculation of <see cref="MatchingAlgorithm.MatchingBase">matching algorithm</see>.
/// </summary>
/// <typeparam name="TParameters">The type of matching parameters.</typeparam>
/// <typeparam name="TResult">The type of result.</typeparam>
public interface IMatchingCalculator<in TParameters, TResult>
    where TParameters : MatchingParametersBase
    where TResult : MatchingResultSummaryBase
{
    Task<TResult> Calculate(TParameters parameters, HeatingSystem heatingSystem);
}
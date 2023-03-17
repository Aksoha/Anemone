using Anemone.Algorithms.Models;

namespace Anemone.Algorithms.Matching;


/// <summary>
/// Provides a way to perform calculation of <see cref="MatchingAlgorithm.MatchingBase">matching algorithm</see>.
/// </summary>
/// <typeparam name="TParameters">The type of matching parameters.</typeparam>
/// <typeparam name="TResult">The type of result.</typeparam>
public interface IMatchingCalculator<in TParameters, out TResult>
    where TParameters : MatchingParameterBase 
    where TResult : MatchingResultBase
{
    TResult Calculate(TParameters parameters, HeatingSystem heatingSystem);
}
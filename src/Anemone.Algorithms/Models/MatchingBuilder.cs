using MatchingAlgorithm;
using RepositoryHeatingSystem = Anemone.Repository.HeatingSystemData.HeatingSystem;
using AlgorithmLlcParameters = MatchingAlgorithm.Llc.LlcMatchingParameter;

namespace Anemone.Algorithms.Models;


/// <summary>
/// Builds <see cref="MatchingBase"/> algorithm class.
/// </summary>
/// <typeparam name="TAlgorithm">The type of matching algorithm.</typeparam>
/// <typeparam name="TParameters">The type of parameters.</typeparam>
public interface IMatchingBuilder<out TAlgorithm, in TParameters> where TAlgorithm : MatchingBase where TParameters : MatchingParameterBase
{
    TAlgorithm Build(TParameters parameters, RepositoryHeatingSystem heatingSystemData);
}
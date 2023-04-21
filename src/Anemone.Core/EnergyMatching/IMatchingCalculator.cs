using System.Threading.Tasks;
using Anemone.Core.Common.Entities;
using Anemone.Core.EnergyMatching.Parameters;
using Anemone.Core.EnergyMatching.Results;

namespace Anemone.Core.EnergyMatching;

public interface IMatchingCalculator<in TParameter, TResult>
    where TParameter : MatchingParametersBase
    where TResult : MatchingResultSummaryBase
{
    Task<TResult> Calculate(TParameter parameters, HeatingSystem heatingSystem);
}
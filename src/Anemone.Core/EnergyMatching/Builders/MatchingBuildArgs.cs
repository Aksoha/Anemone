using Anemone.Core.EnergyMatching.Parameters;

namespace Anemone.Core.EnergyMatching.Builders;

public class MatchingBuildArgs<TParameter> : MatchingBuildArgsBase where TParameter : MatchingParametersBase
{
    public required TParameter Parameter { get; set; }
}
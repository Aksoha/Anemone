using Anemone.Core.Common.Entities;
using Anemone.Core.EnergyMatching;
using Anemone.Core.EnergyMatching.Builders;
using Anemone.Core.EnergyMatching.Parameters;
using Anemone.Core.EnergyMatching.Results;
using Anemone.Infrastructure.EnergyMatching.Builders;
using MatchingAlgorithm.Llc;
using SolutionNotFoundException = MatchingAlgorithm.SolutionNotFoundException;

namespace Anemone.Infrastructure.EnergyMatching;

internal class LlcMatchingCalculator : ILlcMatchingCalculator
{
    public LlcMatchingCalculator(ILlcMatchingBuilder builder)
    {
        Builder = builder;
    }

    private ILlcMatchingBuilder Builder { get; }

    public async Task<LlcMatchingResultSummary> Calculate(LlcMatchingParameters parameter, HeatingSystem heatingSystem)
    {
        var algorithm = CreateMatchingAlgorithm(parameter, heatingSystem);

        try
        {
            var results = await Solve(algorithm);
            var output = ConvertResult(results);
            return new LlcMatchingResultSummary(output, results.First().TurnRatio);
        }
        catch (SolutionNotFoundException e)
        {
            throw new Anemone.Core.EnergyMatching.SolutionNotFoundException(null, e);
        }
    }

    private static IEnumerable<LlcMatchingResultPoint> ConvertResult(LlcMatchingResult[] results)
    {
        return results.Select(x => new LlcMatchingResultPoint
        {
            Resistance = x.Resistance,
            Reactance = x.Reactance,
            Frequency = x.Frequency,
            Temperature = x.Temperature,
            Voltage = x.Voltage,
            Current = x.Current,
            Power = x.Power,
            Capacitance = x.Capacitance,
            Inductance = x.Inductance
        });
    }

    private static Task<LlcMatchingResult[]> Solve(LlcMatching matching)
    {
        return Task.Run(() => matching.EnergyMatching().ToArray());
    }

    private LlcMatching CreateMatchingAlgorithm(LlcMatchingParameters parameter, HeatingSystem heatingSystem)
    {
        return Builder.Build(new LlcMatchingBuildArgs { Parameter = parameter, HeatingSystem = heatingSystem });
    }
}
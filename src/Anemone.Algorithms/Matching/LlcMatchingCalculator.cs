using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anemone.Algorithms.Builders;
using Anemone.Algorithms.Models;
using Anemone.Repository.HeatingSystemData;
using MatchingAlgorithm.Llc;
using LlcMatchingResult = Anemone.Algorithms.Models.LlcMatchingResult;

namespace Anemone.Algorithms.Matching;

public class LlcMatchingCalculator : ILlcMatchingCalculator
{
    private ILlcMatchingBuilder Builder { get; }

    public LlcMatchingCalculator(ILlcMatchingBuilder builder)
    {
        Builder = builder;
    }
    
    public async Task<LlcMatchingResult> Calculate(LlcMatchingParameters parameter, HeatingSystem heatingSystem)
    {
        var algorithm = CreateMatchingAlgorithm(parameter, heatingSystem);
        var results = await Solve(algorithm);
        
        var output = ConvertResult(results);
        return new LlcMatchingResult(output, results.First().TurnRatio);
    }

    private static IEnumerable<LlcMatchingResultPoint> ConvertResult(MatchingAlgorithm.Llc.LlcMatchingResult[] results)
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
            Inductance = x.Inductance,
        });
    }

    private static Task<MatchingAlgorithm.Llc.LlcMatchingResult[]> Solve(LlcMatching matching)
    {
       return Task.Run(() =>  matching.EnergyMatching().ToArray());
    }

    private LlcMatching CreateMatchingAlgorithm(LlcMatchingParameters parameter, HeatingSystem heatingSystem)
    {
        return Builder.Build(new LlcMatchingBuildArgs {Parameter = parameter, HeatingSystem = heatingSystem});
    }
}
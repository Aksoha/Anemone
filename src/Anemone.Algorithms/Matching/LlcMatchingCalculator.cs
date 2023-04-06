using System.Linq;
using Anemone.Algorithms.Builders;
using Anemone.Algorithms.Models;
using Anemone.Repository.HeatingSystemData;

namespace Anemone.Algorithms.Matching;

public class LlcMatchingCalculator : ILlcMatchingCalculator
{
    private ILlcMatchingBuilder Builder { get; }

    public LlcMatchingCalculator(ILlcMatchingBuilder builder)
    {
        Builder = builder;
    }
    
    public LlcMatchingResult Calculate(LlcMatchingParameter parameter, HeatingSystem heatingSystem)
    {
        var matching = Builder.Build(new LlcMatchingBuildArgs {Parameter = parameter, HeatingSystem = heatingSystem});
        var results = matching.EnergyMatching().ToArray();
        
        var output = results.Select(x => new LlcMatchingResultPoint
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
        return new LlcMatchingResult(output, results.First().TurnRatio);
    }
}
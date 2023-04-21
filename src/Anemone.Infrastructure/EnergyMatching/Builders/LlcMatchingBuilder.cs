using Anemone.Core.Common.Entities;
using Anemone.Core.Common.Extensions;
using Anemone.Core.EnergyMatching.Builders;
using Anemone.Core.EnergyMatching.Parameters;
using MatchingAlgorithm;
using MatchingAlgorithm.Llc;
using HeatingSystem = MatchingAlgorithm.HeatingSystem;

namespace Anemone.Infrastructure.EnergyMatching.Builders;

internal class LlcMatchingBuilder : ILlcMatchingBuilder
{
    public LlcMatching Build(LlcMatchingBuildArgs args)
    {
        var hs = ConvertToAlgorithmHeatingSystem(args.HeatingSystem);
        var topology = BuildLlcTopology(hs);
        var p = BuildLlcParameters(args.Parameter);

        return args.Parameter.VariableInductance switch
        {
            true => new LlcActiveMatching(topology, p),
            false => new LlcPassiveMatching(topology, p)
        };
    }

    private HeatingSystem ConvertToAlgorithmHeatingSystem(Core.Common.Entities.HeatingSystem argsHeatingSystem)
    {
        var frequency = argsHeatingSystem.HeatingSystemPoints.Where(p => p.Type == HeatingSystemPointType.Frequency)
            .Select(val => new HeatingSystemData
                { Key = val.TypeValue, Resistance = val.Resistance, Inductance = val.Inductance });
        var temperature = argsHeatingSystem.HeatingSystemPoints.Where(p => p.Type == HeatingSystemPointType.Temperature)
            .Select(val => new HeatingSystemData
                { Key = val.TypeValue, Resistance = val.Resistance, Inductance = val.Inductance });


        return new HeatingSystem(frequency, temperature);
    }

    private LlcMatchingParameter BuildLlcParameters(LlcMatchingParameters parameter)
    {
        // disabled nullability warning, parameters should be null checked by the Validator
#pragma warning disable CS8629
        var capacitance = EnumerableRangeExtensions.CreateRange((double)parameter.CapacitanceMin,
            (double)parameter.CapacitanceMax, (double)parameter.CapacitanceStep);
        var inductance = EnumerableRangeExtensions.CreateRange((double)parameter.InductanceMin,
            (double)parameter.InductanceMax, (double)parameter.InductanceStep);
        var frequency = EnumerableRangeExtensions.CreateRange((double)parameter.FrequencyMin, (double)parameter.FrequencyMax,
            (double)parameter.FrequencyStep);
        var temperature = EnumerableRangeExtensions.CreateRange((double)parameter.TemperatureMin,
            (double)parameter.TemperatureMax, (double)parameter.TemperatureStep);
        var turnRatio = EnumerableRangeExtensions.CreateRange((double)parameter.TurnRatioMin, (double)parameter.TurnRatioMax,
            (double)parameter.TurnRatioStep);

        var output = new LlcMatchingParameter
        {
            Capacitance = capacitance,
            Inductance = inductance,
            Frequency = frequency,
            Temperature = temperature,
            CurrentLimit = (double)parameter.Current,
            ExpectedPower = (double)parameter.Power,
            TurnRatio = turnRatio,
            VoltageLimit = (double)parameter.Voltage,
            AllowPartialCompensation = false
        };
#pragma warning restore CS8629
        return output;
    }

    private ILlcTopology BuildLlcTopology(HeatingSystem data)
    {
        return new LlcTopology(data);
    }
}
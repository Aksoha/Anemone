using System.Linq;
using Anemone.Algorithms.Models;
using Anemone.Repository.HeatingSystemData;
using FluentValidation;
using MatchingAlgorithm;
using MatchingAlgorithm.Llc;
using HeatingSystem = MatchingAlgorithm.HeatingSystem;

namespace Anemone.Algorithms.Builders;

public class LlcMatchingBuilder : ILlcMatchingBuilder
{
    private IValidator<LlcMatchingBuildArgs> Validator { get; }

    public LlcMatchingBuilder(IValidator<LlcMatchingBuildArgs> validator)
    {
        Validator = validator;
    }

    public LlcMatching Build(LlcMatchingBuildArgs args)
    {

        Validator.ValidateAndThrow(args);
        var hs = ConvertToAlgorithmHeatingSystem(args.HeatingSystem);
        var topology = BuildLlcTopology(hs);
        var p = BuildLlcParameters(args.Parameter);

        return args.Parameter.VariableInductance switch
        {
            true => new LlcActiveMatching(topology, p),
            false => new LlcPassiveMatching(topology, p)
        };
    }

    private HeatingSystem ConvertToAlgorithmHeatingSystem(Repository.HeatingSystemData.HeatingSystem argsHeatingSystem)
    {
        var frequency = argsHeatingSystem.HeatingSystemPoints.Where(p => p.Type == HeatingSystemPointType.Frequency)
            .Select(val => new HeatingSystemData {Key = val.TypeValue, Resistance = val.Resistance, Inductance = val.Inductance});
        var temperature = argsHeatingSystem.HeatingSystemPoints.Where(p => p.Type == HeatingSystemPointType.Temperature)
            .Select(val => new HeatingSystemData {Key = val.TypeValue, Resistance = val.Resistance, Inductance = val.Inductance});


        return new HeatingSystem(frequency, temperature);
    }

    private MatchingAlgorithm.Llc.LlcMatchingParameter BuildLlcParameters(LlcMatchingParameters parameter)
    {
        // disabled nullability warning, parameters should be null checked by the Validator
#pragma warning disable CS8629
        var capacitance = EnumerableExtensions.CreateRange((double)parameter.CapacitanceMin, (double)parameter.CapacitanceMax, (double)parameter.CapacitanceStep);
        var inductance =  EnumerableExtensions.CreateRange((double)parameter.InductanceMin, (double)parameter.InductanceMax, (double)parameter.InductanceStep);
        var frequency =   EnumerableExtensions.CreateRange((double)parameter.FrequencyMin, (double)parameter.FrequencyMax, (double)parameter.FrequencyStep);
        var temperature = EnumerableExtensions.CreateRange((double)parameter.TemperatureMin, (double)parameter.TemperatureMax, (double)parameter.TemperatureStep);
        var turnRatio =   EnumerableExtensions.CreateRange((double)parameter.TurnRatioMin, (double)parameter.TurnRatioMax, (double)parameter.TurnRatioStep);
        
        var output = new MatchingAlgorithm.Llc.LlcMatchingParameter
        {
            Capacitance = capacitance,
            Inductance = inductance,
            Frequency = frequency,
            Temperature = temperature,
            CurrentLimit = (double)parameter.Current,
            ExpectedPower = (double)parameter.Power,
            TurnRatio = turnRatio,
            VoltageLimit = (double)parameter.Voltage,
            AllowPartialCompensation = false,
        };
#pragma warning restore CS8629
        return output;
    }
    private ILlcTopology BuildLlcTopology(MatchingAlgorithm.HeatingSystem data)
    {
        return new LlcTopology(data);
    }
}
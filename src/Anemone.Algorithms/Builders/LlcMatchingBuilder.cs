using System;
using System.Collections.Generic;
using Anemone.Repository.HeatingSystemData;
using FluentValidation;
using MatchingAlgorithm;
using MatchingAlgorithm.Llc;
using HeatingSystem = Anemone.Algorithms.Models.HeatingSystem;
using LlcMatchingParameter = Anemone.Algorithms.Models.LlcMatchingParameter;

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
        
        var topology = BuildLlcTopology(args.HeatingSystem);
        var p = BuildLlcParameters(args.Parameter);

        return args.Parameter.VariableInductance switch
        {
            true => new LlcActiveMatching(topology, p),
            false => new LlcPassiveMatching(topology, p)
        };
    }
    private MatchingAlgorithm.Llc.LlcMatchingParameter BuildLlcParameters(LlcMatchingParameter parameter)
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
    private static MatchingAlgorithm.HeatingSystem BuildHeatingSystem(HeatingSystem data)
    {
        var frequency = new List<HeatingSystemData>();
        var temperature = new List<HeatingSystemData>();
        
        foreach (var point in data.Points)
        {
            switch (point.Type)
            {
                case HeatingSystemPointType.Frequency :
                    frequency.Add(new HeatingSystemData {Key = point.TypeValue, Resistance = point.Resistance, Inductance = point.Inductance});
                    break;
                case HeatingSystemPointType.Temperature :
                    temperature.Add(new HeatingSystemData {Key = point.TypeValue, Resistance = point.Resistance, Inductance = point.Inductance});
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(point.Type));
            }
        }
        return new MatchingAlgorithm.HeatingSystem(frequency, temperature);
    }
    private ILlcTopology BuildLlcTopology(HeatingSystem data)
    {
        var heatingData = BuildHeatingSystem(data);
        return BuildLlcTopology(heatingData);
    }
    private ILlcTopology BuildLlcTopology(MatchingAlgorithm.HeatingSystem data)
    {
        return new LlcTopology(data);
    }
}
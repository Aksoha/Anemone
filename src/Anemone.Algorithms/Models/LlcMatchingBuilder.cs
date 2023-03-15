using System;
using System.Collections.Generic;
using Anemone.Repository.HeatingSystemData;
using MatchingAlgorithm;
using MatchingAlgorithm.Llc;
using HeatingSystem = Anemone.Repository.HeatingSystemData.HeatingSystem;

namespace Anemone.Algorithms.Models;

public interface ILlcMatchingBuilder : IMatchingBuilder<LlcMatching, LlcAlgorithmParameters>
{
    
}

public class LlcMatchingBuilder : ILlcMatchingBuilder
{

    public LlcMatching Build(LlcAlgorithmParameters parameters, HeatingSystem heatingSystemData)
    {
        var topology = BuildLlcTopology(heatingSystemData);
        var p = BuildLlcParameters(parameters);

        return parameters.VariableInductance switch
        {
            true => new LlcActiveMatching(topology, p),
            false => new LlcPassiveMatching(topology, p)
        };
    }
    private static LlcMatchingParameter BuildLlcParameters(LlcAlgorithmParameters parameters)
    {
        var capacitance = CreateList(parameters.CapacitanceMin, parameters.CapacitanceMax, parameters.CapacitanceStep);
        var inductance =  CreateList(parameters.InductanceMin, parameters.InductanceMax, parameters.InductanceStep);
        var frequency =   CreateList(parameters.FrequencyMin, parameters.FrequencyMax, parameters.FrequencyStep);
        var temperature = CreateList(parameters.TemperatureMin, parameters.TemperatureMax, parameters.TemperatureStep);
        var turnRatio =   CreateList(parameters.TurnRatioMin, parameters.TurnRatioMax, parameters.TurnRatioStep);
        
        var output = new LlcMatchingParameter
        {
            Capacitance = capacitance,
            Inductance = inductance,
            Frequency = frequency,
            Temperature = temperature,
            CurrentLimit = parameters.Current,
            ExpectedPower = parameters.Power,
            TurnRatio = turnRatio,
            VoltageLimit = parameters.Voltage,
            AllowPartialCompensation = false,
        };

        return output;
    }
    private static MatchingAlgorithm.HeatingSystem BuildHeatingSystem(HeatingSystem data)
    {
        var frequency = new List<HeatingSystemData>();
        var temperature = new List<HeatingSystemData>();
        
        foreach (var point in data.HeatingSystemPoints)
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
    private static IEnumerable<double> CreateList(double min, double max, double increment)
    {
        if (Equals(min, max))
            throw new ArgumentOutOfRangeException(nameof(min));
        if (increment <= 0)
            throw new ArgumentOutOfRangeException(nameof(increment));
        
        var output = new List<double>();
        for (var i = min; i <= max; i += increment)
        {
            output.Add(i);
        }
        return output;
    }
}
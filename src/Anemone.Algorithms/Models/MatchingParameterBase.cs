using System;

namespace Anemone.Algorithms.Models;

public class MatchingParametersBase : ICloneable
{
    public double? FrequencyMin { get; set; }
    public double? FrequencyMax { get; set; }
    public double? FrequencyStep { get; set; }


    public double? TemperatureMin { get; set; }
    public double? TemperatureMax { get; set; }
    public double? TemperatureStep { get; set; }


    public double? TurnRatioMin { get; set; }
    public double? TurnRatioMax { get; set; }
    public double? TurnRatioStep { get; set; }


    public double? Voltage { get; set; }
    public double? Current { get; set; }
    public double? Power { get; set; }
    public object Clone()
    {
        return MemberwiseClone();
    }
}
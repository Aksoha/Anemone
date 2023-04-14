using System;
using System.Collections.Generic;
using System.Linq;
using Anemone.Core;

namespace Anemone.Algorithms.Models;


public class MatchingResultBase
{
    
}

public class MatchingResult<T> : MatchingResultBase where T : MatchingResultPoint
{
    public T[] Points { get; }
    public double MeanPower { get; }
    public double TurnRatio { get; }
    public double MaxFrequencyDerivative { get; }
    public double MaxPhaseShift { get; }

    public MatchingResult()
    {
        Points = Array.Empty<T>();
    }
    public MatchingResult(IEnumerable<T> points, double turnRatio)
    {
        Points = points as T[] ?? points.ToArray();
        MeanPower = Points.Average(x => x.Power);
        TurnRatio = turnRatio;
        MaxFrequencyDerivative = Points.Derivative(x => x.Temperature, x => x.Frequency).Max();
        MaxPhaseShift = Points.Derivative(x => x.Temperature, x => x.PhaseShift).Max();
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using Anemone.Core;

namespace Anemone.Algorithms.Models;

public class MatchingResultSummaryBase
{
}

public abstract class MatchingResultSummary<TPoint> : MatchingResultSummaryBase where TPoint : MatchingResultPoint
{
    public MatchingResultSummary()
    {
        Points = Array.Empty<TPoint>();
    }

    public MatchingResultSummary(IEnumerable<TPoint> points, double turnRatio)
    {
        Points = points as TPoint[] ?? points.ToArray();
        MeanPower = Points.Average(x => x.Power);
        TurnRatio = turnRatio;
        MaxFrequencyDerivative = Points.Derivative(x => x.Temperature, x => x.Frequency).Max();
        MaxPhaseShift = Points.Derivative(x => x.Temperature, x => x.PhaseShift).Max();
    }

    public TPoint[] Points { get; }
    public double MeanPower { get; }
    public double TurnRatio { get; }
    public double MaxFrequencyDerivative { get; }
    public double MaxPhaseShift { get; }
}
﻿using System.Collections.Generic;
using System.Linq;
using Anemone.Core.Common.Extensions;

namespace Anemone.Core.EnergyMatching.Results;

public class LlcMatchingResultSummary : MatchingResultSummary<LlcMatchingResultPoint>
{
    public LlcMatchingResultSummary()
    {
    }

    public LlcMatchingResultSummary(IEnumerable<LlcMatchingResultPoint> points, double turnRatio) : base(points,
        turnRatio)
    {
        Capacitance = Points.First().Capacitance;
        MaxInductanceDerivative = Points.Derivative(x => x.Temperature, x => x.Inductance).Max();
    }

    public double Capacitance { get; }
    public double MaxInductanceDerivative { get; }
}
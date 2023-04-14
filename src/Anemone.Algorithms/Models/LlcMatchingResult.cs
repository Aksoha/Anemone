using System.Collections.Generic;
using System.Linq;
using Anemone.Core;

namespace Anemone.Algorithms.Models;

public class LlcMatchingResult : MatchingResult<LlcMatchingResultPoint>
{
    public double Capacitance { get; }
    public double MaxInductanceDerivative { get; }

    public LlcMatchingResult()
    {
        
    }
    
    public LlcMatchingResult(IEnumerable<LlcMatchingResultPoint> points, double turnRatio) : base(points, turnRatio)
    {
        Capacitance = Points.First().Capacitance;
        MaxInductanceDerivative = Points.Derivative(x => x.Temperature, x => x.Inductance).Max();
    }
}
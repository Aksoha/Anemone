using System.Collections.Generic;
using System.Linq;
using Anemone.Core;
using MatchingAlgorithm.Llc;

namespace Anemone.Algorithms.Models;

public class MatchingEstimator
{
    public double MeanPower { get; private set; }
    public double TurnRatio { get; private set; }
    public double Capacitance { get; private set; }
    public double MaxFrequencyDerivative { get; private set; }
    public double MaxInductanceDerivative { get; private set; }
    public double MaxPhaseShift { get; private set; }
    
    public void Estimate(IEnumerable<LlcMatchingResult> result)
    {
        var llcMatchingResults = result as LlcMatchingResult[] ?? result.ToArray();
        var firstItem = llcMatchingResults.First();
        MeanPower = llcMatchingResults.Average(x => x.Power);
        
        TurnRatio = firstItem.TurnRatio;
        Capacitance = firstItem.Capacitance;
        MaxFrequencyDerivative = llcMatchingResults.Derivative(x => x.Temperature, x => x.Frequency).Max();
        MaxInductanceDerivative = llcMatchingResults.Derivative(x => x.Temperature, x => x.Inductance).Max();
        MaxPhaseShift = llcMatchingResults.Derivative(x => x.Temperature, x => x.Impedance.Phase).Max();
    }
}
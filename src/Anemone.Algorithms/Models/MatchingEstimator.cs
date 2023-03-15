using System;
using System.Collections.Generic;
using System.Linq;
using MatchingAlgorithm.Llc;

namespace Anemone.Algorithms.Models;

public class MatchingEstimator
{
    public double MeanPower { get; }
    public double TurnRatio { get; }
    public double Capacitance { get; }
    public double MaxFrequencyDerivative { get; }
    public double MaxInductanceDerivative { get; }
    public double MaxPhaseShift { get; }

    public MatchingEstimator(IEnumerable<LlcMatchingResult> result)
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

public static class LinqExtensions
{
    public static IEnumerable<double> Derivative<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selectorX, Func<TSource, double> selectorY)
    {
        var enumerable = source as TSource[] ?? source.ToArray();
        var itemPrevious = enumerable.First();

        source = enumerable.Skip(1);

        foreach (var itemNext in source)
        {
            var itemPreviousX = selectorX(itemPrevious);
            var itemPreviousY = selectorY(itemPrevious);

            var itemNextX = selectorX(itemNext);
            var itemNextY = selectorY(itemNext);

            var derivative = (itemNextY - itemPreviousY) / (itemNextX - itemPreviousX);

            yield return derivative;

            itemPrevious = itemNext;
        }
    }
}
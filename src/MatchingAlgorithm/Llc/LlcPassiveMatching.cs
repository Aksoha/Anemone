using System.Collections.Concurrent;
using System.Diagnostics;

namespace MatchingAlgorithm.Llc;

public class LlcPassiveMatching : LlcMatching, IEnergyMatching<LlcMatchingResult>
{
    public LlcPassiveMatching(ILlcTopology topology, LlcMatchingParameter parameters) : base(topology, parameters)
    {
    }

    public IEnumerable<LlcMatchingResult> EnergyMatching()
    {
        Topology.Capacitance = Capacitance.First();
        Topology.Inductance = Inductance.First();

        var compensationRanges = CompensationRanges();
        if (compensationRanges.Count != 1 || compensationRanges.Count(x => x.FullCompensation) != 1)
            // if this throws check CompensationRanges() method, further code assumes that there is only 1 solution
            throw new NotImplementedException(
                "at this point algorithm is designed to work only with full compensation and only 1 solution");


        var currentCompensationRange = compensationRanges.First();
        var ls = Inductance.First(x => x > currentCompensationRange.MinInductance);

        var resonantFrequency = SelectFrequency(ls, currentCompensationRange.Capacitance);


        return SetMatchingResults(resonantFrequency, ls, currentCompensationRange.Capacitance);
    }

    /// <summary>
    ///     Sets the matching result data.
    /// </summary>
    /// <param name="resonantFrequency">Selected frequencies.</param>
    /// <param name="inductance">Selected serial inductance.</param>
    /// <param name="capacitance">Selected capacitance.</param>
    private IEnumerable<LlcMatchingResult> SetMatchingResults(IReadOnlyList<double> resonantFrequency,
        double inductance,
        double capacitance)
    {
        Topology.Inductance = inductance;
        Topology.Capacitance = capacitance;
        var impedanceResult = (from item in Temperature.Select((value, i) => new { i, value })
            let temperature = item.value
            let frequency = resonantFrequency[item.i]
            select Topology.Impedance(frequency, temperature)).ToList();

        var turnRatio = SelectTurnRatio(impedanceResult);

        return (from item in Temperature.Select((t, i) => new { i, value = t })
            let temperature = item.value
            let frequency = resonantFrequency[item.i]
            let impedance = impedanceResult[item.i]
            let voltage = Voltage(ExpectedPower, VoltageLimit, CurrentLimit, NominalResistance, turnRatio, impedance.Magnitude)
            let power = Power(impedance.Real, impedance.Magnitude, voltage, turnRatio)
            let current = Current(impedance.Magnitude, voltage, turnRatio)
            select new LlcMatchingResult
            {
                Capacitance = capacitance,
                Frequency = frequency,
                Temperature = temperature,
                Inductance = inductance,
                Resistance = impedance.Real,
                Reactance = impedance.Imaginary,
                Impedance = impedance,
                Current = current,
                Power = power,
                Voltage = voltage,
                TurnRatio = turnRatio
            }).ToList();
    }

    /// <summary>
    ///     Selects a frequency that minimizes reactance of the system for given <paramref name="inductance" /> and
    ///     <paramref name="capacitance" />.
    /// </summary>
    /// <param name="inductance">Selected serial inductance.</param>
    /// <param name="capacitance">Selected capacitance.</param>
    private List<double> SelectFrequency(double inductance, double capacitance)
    {
        if (Parameters.AllowPartialCompensation)
            throw new NotImplementedException("only full compensation is currently supported");

        // this calculation is quite cpu intensive so it might be better to perform it in parallel
        // however test on data with ~1000 rows shows that it's still quicker to do it with single thread
        

        ConcurrentBag<(FrequencyReactancePair value, long index)> resonantFrequency = new();
        Parallel.ForEach(Temperature, (t, _, index) =>
        {
            var upperResonance = UpperResonanceRegion(t, capacitance);
            if (upperResonance.Count == 0)
                throw new UnreachableException(
                    "can't find resonance for the given set, currently only full compensation is supported");

            var result = upperResonance.MinBy(x => Math.Abs(x.Reactance + 2 * Math.PI * x.Frequency * inductance));
            resonantFrequency.Add(new ValueTuple<FrequencyReactancePair, long>(result, index));
        });
        return resonantFrequency.OrderBy(x => x.index).Select(x => x.value.Frequency).ToList();
    }
}
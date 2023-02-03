// #define PARALLEL_COMPUTING

using System.Collections.Concurrent;
using System.Diagnostics;

namespace MatchingAlgorithm.Llc;

public class LlcMatching : IEnergyMatching<LlcMatchingResult>
{
    private readonly ILlcTopology _topology;

    private readonly LlcMatchingParameter _parameters;
    private double NominalResistance { get; }
    public LlcMatching(ILlcTopology topology, LlcMatchingParameter parameters)
    {
        _topology = topology;
        _parameters = parameters;
        Temperature = new List<double>(parameters.Temperature);
        NominalResistance = VoltageLimit * VoltageLimit / ExpectedPower * 8 / (Math.PI * Math.PI);
    }

    private IEnumerable<double> Frequency => _parameters.Frequency;
    private List<double> Temperature { get; set; }
    private IEnumerable<double> Inductance => _parameters.Inductance;
    private IEnumerable<double> Capacitance => _parameters.Capacitance;
    private double VoltageLimit => _parameters.VoltageLimit;
    private double CurrentLimit => _parameters.CurrentLimit;
    private double ExpectedPower => _parameters.ExpectedPower;


    public IEnumerable<LlcMatchingResult> EnergyMatching()
    {
        _topology.Capacitance = Capacitance.First();
        _topology.Inductance = Inductance.First();
        
        var compensationRanges = CompensationRanges();
        if (compensationRanges.Count != 1 || compensationRanges.Count(x => x.FullCompensation) != 1)
            // if this throws check CompensationRanges() method, further code assumes that there is only 1 solution
            throw new NotImplementedException(
                "at this point algorithm is designed to work only with full compensation and only 1 solution");


        var currentCompensationRange = compensationRanges.First();
        var ls = Inductance.First(x => x > currentCompensationRange.MinInductance);

        var e = ResonantFrequencySetting(ls, currentCompensationRange.Capacitance);

        var results = new List<LlcMatchingResult>();

        foreach (var item in Temperature.Select((value, i) => new { i, value }))
        {
            var temperature = item.value;
            var frequency = e[item.i];

            _topology.Inductance = ls;
            _topology.Capacitance = currentCompensationRange.Capacitance;
            var impedance = _topology.Impedance(frequency, temperature);
            var turnRatio = TurnRatioSetting();
            var voltage = TransformerCalculator.Voltage(ExpectedPower, VoltageLimit, CurrentLimit, NominalResistance,
                turnRatio, impedance.Magnitude);
            var power = TransformerCalculator.Power(impedance.Real, impedance.Magnitude, voltage, turnRatio);
            var current = TransformerCalculator.Current(impedance.Magnitude, VoltageLimit, turnRatio);
            
            results.Add(new LlcMatchingResult
            {
                Capacitance = currentCompensationRange.Capacitance,
                Frequency = frequency,
                Temperature = temperature,
                Inductance = ls,
                Resistance = impedance.Real,
                Reactance = impedance.Imaginary,
                Impedance = impedance,
                Current = current,
                Power = power,
                Voltage = voltage,
                TurnRatio = turnRatio
            });
        }

        return results;
    }


    private double TurnRatioSetting()
    {
        return 15;
    }
    
    private List<double> ResonantFrequencySetting(double inductance, double capacitance)
    {
        if (_parameters.AllowPartialCompensation)
            throw new NotImplementedException("only full compensation is currently supported");

#if PARALLEL_COMPUTING
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
#else
        List<FrequencyReactancePair> resonantFrequency = new();
        foreach (var t in Temperature)
        {
            var upperResonance = UpperResonanceRegion(t, capacitance);
            if (upperResonance.Count == 0)
                throw new UnreachableException(
                    "can't find resonance for the given set, currently only full compensation is supported");

            var result = upperResonance.MinBy(x => Math.Abs(x.Reactance + 2 * Math.PI * x.Frequency * inductance));
            resonantFrequency.Add(result);
        }

        return resonantFrequency.Select(x => x.Frequency).ToList();
#endif
    }


    /// <summary>
    /// Performs capacitance sweep to find <see cref="LlcCompensationResult"/> in which compensation of LLC can be performed by change of serial inductance.
    /// </summary>
    /// <exception cref="NotImplementedException">Thrown when attempting to run algorithm with <see cref="LlcMatchingParameter.AllowPartialCompensation"/> flag.</exception>
    /// <exception cref="SolutionNotFoundException">Thrown when there is no solution for given data set.</exception>
    private List<LlcCompensationResult> CompensationRanges()
    {
        if (_parameters.AllowPartialCompensation)
            throw new NotImplementedException("only full compensation is currently supported");

        // if implementing partial compensation this needs to go
        ReduceCalculationPoints();

        var index = 0;
        var currentCapacitance = Capacitance.ElementAt(index);
        index++;

        var inductance = FullInductanceCompensationRange(currentCapacitance) ?? default;
        while (inductance.Min >= inductance.Max)
        {
            if (index >= Capacitance.Count())
                throw new SolutionNotFoundException("there is no solution for given data set");

            currentCapacitance = Capacitance.ElementAt(index);
            index++;

            inductance = FullInductanceCompensationRange(currentCapacitance) ?? default;
        }

        if (Inductance.First(x => x > inductance.Min) > inductance.Max)
            throw new SolutionNotFoundException(
                $"found solution but it's below specified inductance threshold, required inductance for full compensation: {inductance.Min}");


        if (inductance.Min <= 0 || inductance.Max <= 0)
            throw new UnreachableException("resonance doesn't exist, serial inductance can't be negative!");

        // if implementing partial compensation this needs to go
        RestoreCalculationPoints();

        return new List<LlcCompensationResult>
        {
            new()
            {
                Capacitance = currentCapacitance, MinInductance = inductance.Min,
                MaxInductance = inductance.Max,
                FullCompensation = true
            }
        };
    }

    /// <summary>
    /// Performs a sweep in temperature domain to find min and max values of serial inductance that will allow for full compensation of LLC. 
    /// </summary>
    /// <param name="capacitance">The capacitance for which calculation will be performed.</param>
    /// <returns>Returns a result if it exists, otherwise <see langword="null"/>.</returns>
    private InductanceRange? FullInductanceCompensationRange(double capacitance)
    {
        var output = new InductanceRange { Min = double.MinValue, Max = double.MaxValue };

        foreach (var pairs in Temperature.Select(t => UpperResonanceRegion(t, capacitance)))
        {
            if (pairs.Count == 0)
                return null;

            var minReactancePair = pairs.MinBy(x => x.Reactance);
            var maxReactancePair = pairs.MaxBy(x => x.Reactance);

            var serialInductanceMin = -maxReactancePair.Reactance / (2 * Math.PI * maxReactancePair.Frequency);
            var serialInductanceMax = -minReactancePair.Reactance / (2 * Math.PI * minReactancePair.Frequency);

            if (serialInductanceMax < output.Max)
                output.Max = serialInductanceMax;

            if (serialInductanceMin > output.Min)
                output.Min = serialInductanceMin;
        }

        return output;
    }


    /// <summary>
    ///     Performs a calculation in frequency domain to find all values of frequencies (for given
    ///     <paramref name="temperature" />) for which LLC system is in upper resonance range.
    /// </summary>
    /// <param name="temperature">The temperature of <see cref="HeatingSystem">heating system</see>.</param>
    /// <param name="capacitance">The capacitance for which calculation will be performed.</param>
    /// <returns>A list which contains pairs of frequency and parallel reactance in upper resonance region.</returns>
    /// <remarks>
    ///     An upper resonance is a subsection of <see cref="CapacitiveRegion" /> where character of LLC impedance is
    ///     slowly shifting towards inductive. This region starts from a place where parallel inductance has the lowest value.
    ///     This region is characteristic for its gentle increase of parallel reactance which makes it easy to perform
    ///     impedance compensation with serial inductance.
    /// </remarks>
    private List<FrequencyReactancePair> UpperResonanceRegion(double temperature, double capacitance)
    {
        var capacitiveRange = CapacitiveRegion(temperature, capacitance);
        if (capacitiveRange.Count == 0)
            return capacitiveRange;

        var min = capacitiveRange.MinBy(x => x.Reactance);
        var index = capacitiveRange.IndexOf(min);

        if (index == 0)
            return capacitiveRange;

        capacitiveRange.RemoveRange(0, index);
        return capacitiveRange;
    }


    /// <summary>
    ///     Performs a calculation in frequency domain to find all values of frequencies (for given
    ///     <paramref name="temperature" />) for which LLC system is in capacitive range.
    /// </summary>
    /// <param name="temperature">The temperature of <see cref="HeatingSystem">heating system</see>.</param>
    /// <param name="capacitance">The capacitance for which calculation will be performed.</param>
    /// <returns>A list which contains pairs of frequency and parallel reactance in capacitive region.</returns>
    /// <remarks>
    ///     An capacitive region is a state where LLC system has a negative parallel reactance.
    ///     In this region compensation (zeroing impedance) can be performed by a change of serial inductance.
    /// </remarks>
    private List<FrequencyReactancePair> CapacitiveRegion(double temperature, double capacitance)
    {
        _topology.Capacitance = capacitance;
        var results = (from f in Frequency
            let xp = _topology.ParallelReactance(f, temperature)
            where xp < 0
            select new FrequencyReactancePair { Frequency = f, Reactance = xp }).ToList();
        return results;
    }


    /// <summary>
    /// Restores all data reduced by <see cref="ReduceCalculationPoints"/>.
    /// </summary>
    private void RestoreCalculationPoints()
    {
        Temperature = _parameters.Temperature.ToList();
    }


    /// <summary>
    /// Reduces the number of points required to perform initial sweep for <see cref="FullInductanceCompensationRange"/>.
    /// </summary>
    private void ReduceCalculationPoints()
    {
        var f = Frequency.First();
        var min = _parameters.Temperature.MinBy(t => _topology.Reactance(f, t));
        var max = _parameters.Temperature.MaxBy(t => _topology.Reactance(f, t));
        Temperature = new List<double>
        {
            _parameters.Temperature.First(),
            max,
            min,
            _parameters.Temperature.Last()
        };
    }
}
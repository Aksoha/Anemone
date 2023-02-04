using System.Diagnostics;

namespace MatchingAlgorithm.Llc;

public class LlcMatching : Matching<ILlcTopology, LlcMatchingParameter>
{
    protected LlcMatching(ILlcTopology topology, LlcMatchingParameter parameters)
        : base(topology, parameters)
    {
        Temperature = new List<double>(parameters.Temperature);
    }

    // private List<double> Temperature { get; set; }
    protected IEnumerable<double> Inductance => Parameters.Inductance;
    protected IEnumerable<double> Capacitance => Parameters.Capacitance;
    protected new List<double> Temperature { get; set; }


    /// <summary>
    ///     Performs capacitance sweep to find <see cref="LlcCompensationResult" /> in which compensation of LLC can be
    ///     performed by change of serial inductance.
    /// </summary>
    /// <exception cref="NotImplementedException">
    ///     Thrown when attempting to run algorithm with
    ///     <see cref="LlcMatchingParameter.AllowPartialCompensation" /> flag.
    /// </exception>
    /// <exception cref="SolutionNotFoundException">Thrown when there is no solution for given data set.</exception>
    protected List<LlcCompensationResult> CompensationRanges()
    {
        if (Parameters.AllowPartialCompensation)
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
    ///     Performs a sweep in temperature domain to find min and max values of serial inductance that will allow for full
    ///     compensation of LLC.
    /// </summary>
    /// <param name="capacitance">The capacitance for which calculation will be performed.</param>
    /// <returns>Returns a result if it exists, otherwise <see langword="null" />.</returns>
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
    protected List<FrequencyReactancePair> UpperResonanceRegion(double temperature, double capacitance)
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
        Topology.Capacitance = capacitance;
        var results = (from f in Frequency
            let xp = Topology.ParallelReactance(f, temperature)
            where xp < 0
            select new FrequencyReactancePair { Frequency = f, Reactance = xp }).ToList();
        return results;
    }


    /// <summary>
    ///     Restores all data reduced by <see cref="ReduceCalculationPoints" />.
    /// </summary>
    private void RestoreCalculationPoints()
    {
        Temperature = Parameters.Temperature.ToList();
    }


    /// <summary>
    ///     Reduces the number of points required to perform initial sweep for <see cref="FullInductanceCompensationRange" />.
    /// </summary>
    private void ReduceCalculationPoints()
    {
        var f = Frequency.First();
        var min = Parameters.Temperature.MinBy(t => Topology.Reactance(f, t));
        var max = Parameters.Temperature.MaxBy(t => Topology.Reactance(f, t));
        Temperature = new List<double>
        {
            Parameters.Temperature.First(),
            max,
            min,
            Parameters.Temperature.Last()
        };
    }
}
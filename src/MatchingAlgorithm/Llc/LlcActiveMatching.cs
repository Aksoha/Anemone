using System.Collections.Concurrent;
using System.Numerics;
using MatchingAlgorithm.Extensions;

namespace MatchingAlgorithm.Llc;

public class LlcActiveMatching : LlcMatching, IEnergyMatching<LlcMatchingResult>
{
    public LlcActiveMatching(ILlcTopology topology, LlcMatchingParameter parameters) : base(topology, parameters)
    {
    }

    public IEnumerable<LlcMatchingResult> EnergyMatching()
    {
        // ALGORITHM
        // currently supporting only 1 solution, requires resonance on entire range
        // 1 - find capacitance that will give resonance in given frequency range
        // 2 - perform a selection of resistance (try to keep resistance constant and really close to nominal resistance)
        //     this calculation is the main part of matching and it can be quite taxing, any performance improvements should be done here
        // 3 - calculate frequency and serial inductance for given resistance


        Topology.Capacitance = Capacitance.First();
        Topology.Inductance = Inductance.First();

        var compensationRanges = CompensationRanges();
        if (compensationRanges.Count != 1 || compensationRanges.Count(x => x.FullCompensation) != 1)
            // if this throws check CompensationRanges() method, further code assumes that there is only 1 solution
            throw new NotImplementedException(
                "at this point algorithm is designed to work only with full compensation and only 1 solution");

        var currentCompensationRange = compensationRanges.First();

        var resistance = SelectResistance(currentCompensationRange.Capacitance);
        var matchingResult = SelectFrequencyAndInductance(resistance, currentCompensationRange.Capacitance);
        return SetMatchingResults(matchingResult, currentCompensationRange.Capacitance);
    }


    /// <summary>
    ///     Sets the matching result data.
    /// </summary>
    /// <param name="matchingResults">Selected frequency and inductance.</param>
    /// <param name="capacitance">Selected capacitance.</param>
    private IEnumerable<LlcMatchingResult> SetMatchingResults(
        IReadOnlyList<(double frequency, double inductance)> matchingResults,
        double capacitance)
    {
        Topology.Capacitance = capacitance;

        var impedanceList = new List<Complex>();
        for (var i = 0; i < Temperature.Count; i++)
        {
            var frequency = matchingResults[i].frequency;
            var inductance = matchingResults[i].inductance;
            var temperature = Temperature[i];

            Topology.Inductance = inductance;
            impedanceList.Add(Topology.Impedance(frequency, temperature));
        }

        var turnRatio = SelectTurnRatio(impedanceList);


        var output = new List<LlcMatchingResult>();

        for (var i = 0; i < Temperature.Count; i++)
        {
            var frequency = matchingResults[i].frequency;
            var temperature = Temperature[i];
            var inductance = matchingResults[i].inductance;
            var impedance = impedanceList[i];
            var resistance = impedance.Real;
            var reactance = impedance.Imaginary;
            var voltage = Voltage(ExpectedPower, VoltageLimit, CurrentLimit, NominalResistance, turnRatio,
                impedance.Magnitude);
            var power = Power(resistance, impedance.Magnitude, voltage, turnRatio);
            var current = Current(impedance.Magnitude, voltage, turnRatio);

            output.Add(new LlcMatchingResult
            {
                Capacitance = capacitance,
                Frequency = frequency,
                Temperature = temperature,
                Inductance = inductance,
                Resistance = resistance,
                Reactance = reactance,
                Impedance = impedance,
                Current = current,
                Power = power,
                Voltage = voltage,
                TurnRatio = turnRatio
            });
        }

        return output;
    }


    /// <summary>
    /// Selects resonant frequency and serial inductance for given <paramref name="resistance"/> and <paramref name="capacitance"/>.
    /// </summary>
    /// <param name="resistance">Selected resistance.</param>
    /// <param name="capacitance">Selected capacitance.</param>
    private List<(double frequency, double inductance)> SelectFrequencyAndInductance(IList<double> resistance,
        double capacitance)
    {
        Topology.Capacitance = capacitance;
        
        ConcurrentBag<(double frequency, double inductance, long index)> output = new();
        Parallel.ForEach(Temperature, (temperature, _, index) =>
        {
            var dRr = double.MaxValue;
            var dRrIdx = 0;
            for (var j = 0; j < Frequency.Count(); j++)
            {
                // find frequency for the given resistance
                var r = Topology.Resistance(Frequency.ElementAt(j), temperature);
                var dR = Math.Abs(r - resistance[(int)index]);
                if (!(dRr > dR)) continue;
                dRr = dR;
                dRrIdx = j;
            }

            // calculate serial inductance and set resonant frequency
            var selectedFrequency = Frequency.ElementAt(dRrIdx);
            var parallelReactance = Topology.ParallelReactance(selectedFrequency, temperature);
            var serialInductance = ReactanceToInductance(new FrequencyReactancePair
                { Frequency = selectedFrequency, Reactance = parallelReactance });

            output.Add((selectedFrequency, serialInductance, index));
        });

        return  output.OrderBy(x => x.index)
            .Select(x => new ValueTuple<double, double>(x.frequency, x.inductance)).ToList();
    }

    
    /// <summary>
    /// Selects a resistance that maximizes energy.
    /// </summary>
    /// <param name="capacitance">Selected capacitance.</param>
    private List<double> SelectResistance(double capacitance)
    {
        var list = ResistanceRangeWithResonance(capacitance);

        var resistanceMaxList = list.Select(x => x.Rmax).ToList();
        var resistanceMinList = list.Select(x => x.Rmin).ToList();

        var resistanceMaxMin = resistanceMaxList.Max();
        var resistanceMaxMax = resistanceMaxList.Min();
        var resistanceMinMax = resistanceMinList.Max();
        var resistanceMinMin = resistanceMinList.Min();

        var nominalResistanceSecondary = TurnRatio.Select(k => NominalResistance / (k * k)).ToList();

        var output = new List<double>();


        // nominal equivalent resistance
        double reqNominal;


        // resistance can be constant in entire temperature range
        // selecting resistance closest to the nominal resistance
        // selection is performed by calculating nominal resistance on secondary side (nominalResistanceSecondary)
        // for each TurnRatio and selecting the resistance that is closest to one of calculated nominalResistanceSecondary
        if (resistanceMaxMin > resistanceMinMax)
        {
            var dRIdx = nominalResistanceSecondary.MinAbsIdx(resistanceMinMax);
            var dR = nominalResistanceSecondary.ElementAt(dRIdx);

            reqNominal = dR;
            if (reqNominal < resistanceMinMax && reqNominal > resistanceMaxMin)
            {
                // equivalent resistance
                var req = reqNominal;
                var d1 = resistanceMaxList.MinBy(x => Math.Abs(x - req));
                var d2 = resistanceMinList.MinBy(x => Math.Abs(x - req));
                reqNominal = d1 < d2 ? resistanceMaxMin : resistanceMinMax;
            }

            output.AddRange(Temperature.Select(_ => reqNominal));
            return output;
        }


        // resistance can't be constant in entire temperature range
        // performing sweep from top to bottom of resistance to find values for which resistance is staying mostly the same
        const int steps = 1000;
        var dY = (resistanceMaxMax - resistanceMinMin) / steps;

        var minStd = double.MaxValue;
        double minStdIdx = 0;

        // find resistance range that is the most flat
        for (var i = 0; i < steps; i++)
        {
            reqNominal = resistanceMaxMax - dY * i;
            output.Clear();

            // add constant value of resistance
            for (var j = 0; j < Temperature.Count; j++)
                output.Add(reqNominal);

            SetResistanceToClosestValue(ref output, resistanceMinList, resistanceMaxList);
            var std = output.Std();
            if (!(minStd > std)) continue;
            minStd = std;
            minStdIdx = i;
        }

        // set the value or resistance
        output.Clear();
        reqNominal = resistanceMaxMax - dY * minStdIdx;
        for (var j = 0; j < Temperature.Count; j++)
            output.Add(reqNominal);
        SetResistanceToClosestValue(ref output, resistanceMinList, resistanceMaxList);

        return output;

        void SetResistanceToClosestValue(ref List<double> resistance, IReadOnlyList<double> lowerBound,
            IReadOnlyList<double> upperBound)
        {
            if (resistance.Count != lowerBound.Count || resistance.Count != upperBound.Count)
                throw new ArgumentException("collections must have same size");

            var result = resistance;
            for (var i = 0; i < resistance.Count; i++)
            {
                var r = resistance[i];
                if (r < lowerBound[i])
                    result[i] = lowerBound[i];
                else if (r > upperBound[i])
                    result[i] = upperBound[i];
            }

            resistance = result;
        }
    }

    /// <summary>
    /// Returns an lower and upper range of resistance in temperature domain for given <paramref name="capacitance"/>.
    /// </summary>
    /// <param name="capacitance">Selected capacitance.</param>
    private List<(double Rmin, double Rmax)> ResistanceRangeWithResonance(double capacitance)
    {
        Topology.Capacitance = capacitance;
        
        var output = new ConcurrentBag<(double Rmin, double Rmax, long index)>();
        Parallel.ForEach(Temperature, (t, _, index) =>
        {
            var xp = UpperResonanceRegion(t, capacitance);

            var minResistance = double.MaxValue;
            var minResistanceIdx = 0;

            var maxResistance = double.MinValue;
            var maxResistanceIdx = 0;

            // find min and max resistance for given temperature
            for (var i = 0; i < xp.Count; i++)
            {
                var f = xp.ElementAt(i).Frequency;
                var resistance = Topology.Resistance(f, t);
                if (minResistance > resistance)
                {
                    minResistance = resistance;
                    minResistanceIdx = i;
                }

                if ((maxResistance < resistance) is false) continue;
                maxResistance = resistance;
                maxResistanceIdx = i;
            }

            var xpMax = xp.ElementAt(maxResistanceIdx);
            var xpMin = xp.ElementAt(minResistanceIdx);
            
            // minimal inductance is at maximal parallel reactance
            var lsmin = ReactanceToInductance(xpMax);
            var lsmax = ReactanceToInductance(xpMin);

            AdjustResistance(ref minResistance, ref minResistanceIdx, lsmax, xp, t);
            AdjustResistance(ref maxResistance, ref maxResistanceIdx, lsmin, xp, t);


            output.Add((minResistance, maxResistance, index));

        });
        
        return  output.OrderBy(x => x.index).Select(x => new ValueTuple<double, double>(x.Rmin, x.Rmax)).ToList();
        

        // checks whether inductance is above minimal value
        // if inductance is below min value performs a new calculation of resistance at ls = lsmin = Inductance.Min()
        void AdjustResistance(ref double resistance, ref int index, double ls,
            IReadOnlyCollection<FrequencyReactancePair> reactancePair, double t)
        {
            // above minimal inductance, don't need to adjust anything
            var minDifference = double.MaxValue;
            if ((ls < Inductance.First()) is false) return;

            
            // can't use this inductance, it's too low, increase it
            ls = Inductance.First();

            // new inductance requires recalculation of resistance
            for (var i = 0; i < reactancePair.Count; i++)
            {
                var xp = reactancePair.ElementAt(i);
                
                var difference = Math.Abs(xp.Reactance + 2 * Math.PI * xp.Frequency * ls);
                if ((minDifference > difference) is false) continue;
                minDifference = difference;
                index = i;
            }
            resistance = Topology.Resistance(reactancePair.ElementAt(index).Frequency, t);
        }
    }
}
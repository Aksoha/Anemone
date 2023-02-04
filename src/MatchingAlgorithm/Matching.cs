using System.Numerics;

namespace MatchingAlgorithm;

public abstract class Matching<TTopology, TParameters>
    where TTopology : ITopology
    where TParameters : MatchingParameter
{
    protected readonly double NominalResistance;
    protected readonly TParameters Parameters;
    protected readonly TTopology Topology;

    protected Matching(TTopology topology, TParameters parameters)
    {
        Topology = topology;
        Parameters = parameters;
        NominalResistance = VoltageLimit * VoltageLimit / ExpectedPower * 8 / (Math.PI * Math.PI);
    }

    protected IEnumerable<double> Frequency => Parameters.Frequency;
    protected IEnumerable<double> Temperature => Parameters.Temperature;
    protected double VoltageLimit => Parameters.VoltageLimit;
    protected double CurrentLimit => Parameters.CurrentLimit;
    protected double ExpectedPower => Parameters.ExpectedPower;
    protected IEnumerable<double> TurnRatio => Parameters.TurnRatio;

    /// <summary>
    ///     Selects a turn ratio that will maximize the power output.
    /// </summary>
    /// <param name="impedanceEnumerable">The impedance of llc system, calculated on secondary side of the transformer.</param>
    protected double SelectTurnRatio(IEnumerable<Complex> impedanceEnumerable)
    {
        double selectedTurnRatio = 0;
        double meanPowerAtGivenTurnRatio = 0;


        var impedanceArray = impedanceEnumerable.ToArray();

        foreach (var k in TurnRatio)
        {
            var power = (from impedance in impedanceArray
                let magnitude = impedance.Magnitude
                let resistance = impedance.Real
                let voltage =
                    TransformerCalculator.Voltage(ExpectedPower, VoltageLimit, CurrentLimit, NominalResistance, k,
                        magnitude)
                select TransformerCalculator.Power(resistance, magnitude, voltage, k)).ToList();

            var mean = power.Average();

            if (!(mean > meanPowerAtGivenTurnRatio)) continue;
            meanPowerAtGivenTurnRatio = mean;
            selectedTurnRatio = k;
        }


        return selectedTurnRatio;
    }
}
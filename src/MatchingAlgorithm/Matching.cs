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
                    Voltage(ExpectedPower, VoltageLimit, CurrentLimit, NominalResistance, k,
                        magnitude)
                select Power(resistance, magnitude, voltage, k)).ToList();

            var mean = power.Average();

            if (!(mean > meanPowerAtGivenTurnRatio)) continue;
            meanPowerAtGivenTurnRatio = mean;
            selectedTurnRatio = k;
        }


        return selectedTurnRatio;
    }
    
        /// <summary>
    /// Calculate voltage of primary side while retaining current and voltage limits
    /// </summary>
    /// <param name="expectedPower">Load power.</param>
    /// <param name="voltageMax">RMS voltage of primary side.</param>
    /// <param name="currentMax">RMS current of primary side.</param>
    /// <param name="nominalResistance">Resistance of primary side.</param>
    /// <param name="turnRatio">Turn ratio.</param>
    /// <param name="impedance">Impedance of system (secondary side).</param>
        protected static double Voltage(
        double expectedPower,
        double voltageMax,
        double currentMax,
        double nominalResistance,
        double turnRatio,
        double impedance)
    {
        var resistanceMinimal = expectedPower / (currentMax * currentMax) / (turnRatio * turnRatio);
        var nominalSecondaryResistance = TransformerCalculator.ResistanceToSecondary(nominalResistance, turnRatio);

        var voltage = voltageMax * Math.Sqrt(impedance / nominalSecondaryResistance);

        // we are looking at transformer from other side (stepping up voltage) that's why equation is inverted
        if (impedance < resistanceMinimal)
            voltage = currentMax / Math.Sqrt(8 / (Math.PI * Math.PI)) * TransformerCalculator.ResistanceToSecondary(impedance, turnRatio);

        return voltage > voltageMax ? voltageMax : voltage;
    }


    /// <summary>
    /// Calculate load power.
    /// </summary>
    /// <param name="resistance">Resistance of the system on the secondary side.</param>
    /// <param name="impedance">Impedance of the system on the secondary side.</param>
    /// <param name="voltage">RMS voltage on primary side.</param>
    /// <param name="turnRatio">Turn ratio.</param>
    protected static double Power(double resistance, double impedance, double voltage, double turnRatio)
    {
        var current = Current(impedance, voltage, turnRatio);
        var power = current * voltage * resistance / impedance;
        power /= 1.11;
        return power;
    }


    /// <summary>
    /// Calculate current on primary side.
    /// </summary>
    /// <param name="impedance">Impedance of the system on the secondary side.</param>
    /// <param name="voltage">RMS voltage on primary side.</param>
    /// <param name="turnRatio">Turn ratio.</param>
    protected static double Current(double impedance, double voltage, double turnRatio)
    {
        // we are looking at transformer from other side (stepping up voltage) that's why equation is inverted
        return voltage / TransformerCalculator.ResistanceToSecondary(impedance, turnRatio) / 1.11;
    }
}
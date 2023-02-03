namespace MatchingAlgorithm;

public static class TransformerCalculator
{
    /// <summary>
    /// Calculate voltage of primary side while retaining current and voltage limits
    /// </summary>
    /// <param name="expectedPower">Load power.</param>
    /// <param name="voltageMax">RMS voltage of primary side.</param>
    /// <param name="currentMax">RMS current of primary side.</param>
    /// <param name="nominalResistance">Resistance of primary side.</param>
    /// <param name="turnRatio">Turn ratio.</param>
    /// <param name="impedance">Impedance of system (secondary side).</param>
    public static double Voltage(
        double expectedPower,
        double voltageMax,
        double currentMax,
        double nominalResistance,
        double turnRatio,
        double impedance)
    {
        var resistanceMinimal = expectedPower / (currentMax * currentMax) / (turnRatio * turnRatio);
        var nominalSecondaryResistance = nominalResistance / (turnRatio * turnRatio);

        var voltage = voltageMax * Math.Sqrt(impedance / nominalSecondaryResistance);

        if (impedance < resistanceMinimal)
            voltage = currentMax / Math.Sqrt(8 / (Math.PI * Math.PI)) * impedance * turnRatio * turnRatio;

        return voltage > voltageMax ? voltageMax : voltage;
    }


    /// <summary>
    /// Calculate load power.
    /// </summary>
    /// <param name="resistance">Resistance of the system on the secondary side.</param>
    /// <param name="impedance">Impedance of the system on the secondary side.</param>
    /// <param name="voltage">RMS voltage on primary side.</param>
    /// <param name="turnRatio">Turn ratio.</param>
    public static double Power(double resistance, double impedance, double voltage, double turnRatio)
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
    public static double Current(double impedance, double voltage, double turnRatio)
    {
        return voltage / (impedance * turnRatio * turnRatio) / 1.11;
    }
}
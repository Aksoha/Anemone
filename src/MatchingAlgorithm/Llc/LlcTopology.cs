using System.Numerics;

namespace MatchingAlgorithm.Llc;

public class LlcTopology : ILlcTopology
{
    private readonly IHeatingSystem _heatingSystem;

    private readonly Func<double, double, double> _ihsResistance;
    private readonly Func<double, double, double> _ihsReactance;

    public LlcTopology(IHeatingSystem heatingSystem)
    {
        _heatingSystem = heatingSystem;
        _ihsResistance = heatingSystem.Resistance;
        _ihsReactance = heatingSystem.Reactance;
    }

    public double Resistance(double frequency, double temperature)
    {
        var w = AngularFrequency(frequency);
        var r = _heatingSystem.Resistance(frequency, temperature);
        var l = _heatingSystem.Reactance(frequency, temperature);
        
        var req = r /
                  (Math.Pow(1 - w * w * l * Capacitance, 2) +
                   Math.Pow(w * Capacitance * r, 2));

        return req;
    }

    public double Reactance(double frequency, double temperature)
    {
        return AngularFrequency(frequency) * Inductance + ParallelReactance(frequency, temperature);
    }

    public Complex Impedance(double frequency, double temperature)
    {
        return new Complex(Resistance(frequency, temperature), Reactance(frequency, temperature));
    }

    public double Inductance { get; set; }
    public double Capacitance { get; set; }

    public double ParallelReactance(double frequency, double temperature)
    {
        var w = AngularFrequency(frequency);
        var r = _heatingSystem.Resistance(frequency, temperature);
        var l = _heatingSystem.Reactance(frequency, temperature);
        
        var xp =
            (w * l * (1 - w * w * l * Capacitance) - w * Capacitance * r * r) /
            (Math.Pow(1 - w * w * l * Capacitance, 2) + Math.Pow(w * Capacitance * r, 2));

        return xp;
    }

    private static double AngularFrequency(double frequency)
    {
        return 2 * Math.PI * frequency;
    }
}
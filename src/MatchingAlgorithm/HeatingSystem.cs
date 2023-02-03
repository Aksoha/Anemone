using System.Numerics;

namespace MatchingAlgorithm;

public class HeatingSystem : IHeatingSystem
{
    private readonly Dictionary<double, double> _resistanceFrequency;
    private readonly Dictionary<double, double> _inductanceFrequency;
    private readonly Dictionary<double, double> _resistanceTemperature;
    private readonly Dictionary<double, double> _inductanceTemperature;

    public HeatingSystem(IEnumerable<HeatingSystemData> frequency, IEnumerable<HeatingSystemData> temperature)
    {
        var fList = frequency.ToArray();
        var tList = temperature.ToArray();
        _resistanceFrequency = fList.ToDictionary(key => key.Key, value => value.Resistance);
        _inductanceFrequency = fList.ToDictionary(key => key.Key, value => value.Inductance);
        _resistanceTemperature = tList.ToDictionary(key => key.Key, value => value.Resistance);
        _inductanceTemperature = tList.ToDictionary(key => key.Key, value => value.Inductance);
    }

    public double Resistance(double frequency, double temperature)
    {
        return _resistanceFrequency[frequency] * _resistanceTemperature[temperature];
    }

    public double Reactance(double frequency, double temperature)
    {
        return _inductanceFrequency[frequency] * _inductanceTemperature[temperature];
    }

    public Complex Impedance(double frequency, double temperature)
    {
        return new Complex(Resistance(frequency, temperature), Reactance(frequency, temperature));
    }
}
namespace MatchingAlgorithm.Wrapper;

public class HeatingSystem : IDisposable
{
    private nint heatingSystem;
    private HeatingSystemDataWrapper[] Frequency { get; }
    private HeatingSystemDataWrapper[] Temperature { get; }

    public HeatingSystem(IEnumerable<HeatingSystemData> frequency, IEnumerable<HeatingSystemData> temperature)
    {
        Frequency = Cast(frequency);
        Temperature = Cast(temperature);
        heatingSystem =
            HeatingSystemWrapper.CreateHeatingSystem(Frequency, Temperature, Frequency.Length, Temperature.Length);
    }

    public double Resistance(double frequency, double temperature)
    {
        CheckIfExist(Frequency, frequency);
        CheckIfExist(Temperature, temperature);

        return HeatingSystemWrapper.Resistance(heatingSystem, frequency, temperature);
    }

    private void CheckIfExist(HeatingSystemDataWrapper[] property, double value)
    {
        if (property.Any(x => x.Key.Equals(value)) is false)
            throw new ArgumentOutOfRangeException($"the value {value} was not found in the collection");
    }


    public double Inductance(double frequency, double temperature)
    {
        CheckIfExist(Frequency, frequency);
        CheckIfExist(Temperature, temperature);

        return HeatingSystemWrapper.Inductance(heatingSystem, frequency, temperature);
    }


    public double Impedance(double frequency, double temperature)
    {
        CheckIfExist(Frequency, frequency);
        CheckIfExist(Temperature, temperature);

        return HeatingSystemWrapper.Impedance(heatingSystem, frequency, temperature);
    }

    private HeatingSystemDataWrapper[] Cast(IEnumerable<HeatingSystemData> source)
    {
        return source.Select(x => new HeatingSystemDataWrapper
            { Key = x.Key, Inductance = x.Inductance, Resistance = x.Resistance }).ToArray();
    }

    public void Dispose()
    {
        HeatingSystemWrapper.DisposeHeatingSystem(heatingSystem);
        heatingSystem = nint.Zero;
    }
}
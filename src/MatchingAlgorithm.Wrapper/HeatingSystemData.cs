namespace MatchingAlgorithm.Wrapper;

public struct HeatingSystemData
{
    public double Key;
    public double Resistance;
    public double Inductance;

    public HeatingSystemData(double key, double resistance, double inductance)
    {
        Key = key;
        Resistance = resistance;
        Inductance = inductance;
    }
}
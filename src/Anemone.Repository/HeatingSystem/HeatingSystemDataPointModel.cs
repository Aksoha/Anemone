using System;

namespace Anemone.Repository.HeatingSystem;

/// <summary>
///     A value of heating system for given operating point (<see cref="Key" /> which can refer to frequency or temperature
///     data).
/// </summary>
public struct HeatingSystemDataPointModel
{
    public double Key;
    public double Resistance;
    public double Inductance;

    public HeatingSystemDataPointModel(double key, double resistance, double inductance)
    {
        Key = key;
        Resistance = resistance;
        Inductance = inductance;
    }

    public static bool operator ==(HeatingSystemDataPointModel lhs, HeatingSystemDataPointModel rhs)
    {
        return lhs.Key == rhs.Key && lhs.Resistance == rhs.Resistance && lhs.Inductance == rhs.Inductance;
    }

    public static bool operator !=(HeatingSystemDataPointModel lhs, HeatingSystemDataPointModel rhs)
    {
        return lhs.Key != rhs.Key && lhs.Resistance != rhs.Resistance && lhs.Inductance != rhs.Inductance;
    }

    public bool Equals(HeatingSystemDataPointModel other)
    {
        return Key.Equals(other.Key) && Resistance.Equals(other.Resistance) && Inductance.Equals(other.Inductance);
    }

    public override bool Equals(object? obj)
    {
        return obj is HeatingSystemDataPointModel other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Key, Resistance, Inductance);
    }
}
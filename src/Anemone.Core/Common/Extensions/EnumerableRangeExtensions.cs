using System;
using System.Collections.Generic;

namespace Anemone.Core.Common.Extensions;

public static class EnumerableRangeExtensions
{
    /// <summary>
    ///     Creates a range in ascending order.
    /// </summary>
    /// <param name="min">starting value.</param>
    /// <param name="max">ending value.</param>
    /// <param name="increment">step size.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when <paramref name="min" /> == <paramref name="max" /> or
    ///     <paramref name="increment" /> is zero.
    /// </exception>
    public static IEnumerable<double> CreateRange(double min, double max, double increment)
    {
        if (Equals(min, max))
            throw new ArgumentOutOfRangeException(nameof(min));
        if (increment <= 0)
            throw new ArgumentOutOfRangeException(nameof(increment));

        var output = new List<double>();
        for (var i = min; i <= max; i += increment) output.Add(i);
        return output;
    }
}
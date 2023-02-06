

namespace MatchingAlgorithm.Extensions;

public static class LinqExtensions
{
    public static int MinAbsIdx(this IEnumerable<double> source, double value)
    {
        var idx = 0;
        var returnIdx = 0;
        var lastAbs = double.MaxValue;

        foreach (var item in source)
        {
            var currentAbs = Math.Abs(item - value);

            if (currentAbs < lastAbs)
            {
                lastAbs = currentAbs;
                returnIdx = idx;
            }

            idx++;
        }

        return returnIdx;
    }

    public static double Std(this IEnumerable<double> source)
    {
        var enumerable = source.ToArray();
        var avg = enumerable.Average();
        return Math.Sqrt(enumerable.Average(x => Math.Pow(x - avg, 2)));
    }
}
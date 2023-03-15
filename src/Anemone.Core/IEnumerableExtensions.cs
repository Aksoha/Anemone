using System;
using System.Collections.Generic;
using System.Linq;

namespace Anemone.Core;

public static class EnumerableExtensions
{
    public static IEnumerable<double> Derivative<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selectorX, Func<TSource, double> selectorY)
    {
        var enumerable = source as TSource[] ?? source.ToArray();
        var itemPrevious = enumerable.First();

        source = enumerable.Skip(1);

        foreach (var itemNext in source)
        {
            var itemPreviousX = selectorX(itemPrevious);
            var itemPreviousY = selectorY(itemPrevious);

            var itemNextX = selectorX(itemNext);
            var itemNextY = selectorY(itemNext);

            var derivative = (itemNextY - itemPreviousY) / (itemNextX - itemPreviousX);

            yield return derivative;

            itemPrevious = itemNext;
        }
    }
}
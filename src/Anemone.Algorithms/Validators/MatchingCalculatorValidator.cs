using System;
using System.Collections.Generic;
using System.Linq;
using Anemone.Algorithms.Builders;
using Anemone.Algorithms.Models;
using Anemone.Repository.HeatingSystemData;
using FluentValidation;

namespace Anemone.Algorithms.Validators;

public class MatchingCalculatorValidatorBase<TBuilder, TParameter> : AbstractValidator<TBuilder>
    where TBuilder : MatchingBuildArgs<TParameter>
    where TParameter : MatchingParametersBase
{
    public MatchingCalculatorValidatorBase(IValidator<TParameter> validator)
    {
        RuleFor(x => x.Parameter)
            .Cascade(CascadeMode.Stop)
            .SetValidator(validator)
            .DependentRules(() => { RuleFor(x => x).Must(HaveAllCalculationPoints); });
    }

    private static bool HaveAllCalculationPoints(TBuilder data)
    {
        var parameters = data.Parameter;
        var heatingSystemPoints = data.HeatingSystem.HeatingSystemPoints;

        
        var requiredTemperatures = CreateHashSetFromRange(parameters.TemperatureMin, parameters.TemperatureMax,
            parameters.TemperatureStep);
        var actualTemperatures = SelectPointKeys(heatingSystemPoints, HeatingSystemPointType.Temperature);
        if (requiredTemperatures.IsSubsetOf(actualTemperatures) is false)
            return false;

        var requiredFrequencies =
            CreateHashSetFromRange(parameters.FrequencyMin, parameters.FrequencyMax, parameters.FrequencyStep);
        var actualFrequencies = SelectPointKeys(heatingSystemPoints, HeatingSystemPointType.Frequency);
        if (requiredFrequencies.IsSubsetOf(actualFrequencies) is false)
            return false;

        return true;
    }

    private static HashSet<double> CreateHashSetFromRange(double? min, double? max, double? step)
    {
        // arguments should never be null by this point
        ArgumentNullException.ThrowIfNull(min);
        ArgumentNullException.ThrowIfNull(max);
        ArgumentNullException.ThrowIfNull(step);

        return EnumerableExtensions.CreateRange((double)min,
            (double)max,
            (double)step).ToHashSet();
    }

    private static IEnumerable<double> SelectPointKeys(IEnumerable<HeatingSystemPoint> points,
        HeatingSystemPointType type)
    {
        return from point in points
            where point.Type == type
            select point.TypeValue;
    }
}
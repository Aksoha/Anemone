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
        RuleFor(x => x.Parameter).Cascade(CascadeMode.Stop).SetValidator(validator).DependentRules(() =>
        {
            RuleFor(x => x).Must(HaveAllCalculationPoints);
        });
    }

    private static bool HaveAllCalculationPoints(TBuilder data)
    {
        // disabled nullability warning, parameters should be null checked by the another validator
#pragma warning disable CS8629
        var frequencyParameter = EnumerableExtensions.CreateRange((double)data.Parameter.FrequencyMin,
            (double)data.Parameter.FrequencyMax,
            (double)data.Parameter.FrequencyStep);
        var temperatureParameter = EnumerableExtensions.CreateRange((double)data.Parameter.TemperatureMin,
            (double)data.Parameter.TemperatureMax, (double)data.Parameter.TemperatureStep);
#pragma warning restore CS8629


        var heatingSystemFrequencies = (from p in data.HeatingSystem.HeatingSystemPoints
            where p.Type == HeatingSystemPointType.Frequency
            select p.TypeValue).ToArray();

        var heatingSystemTemperature = (from p in data.HeatingSystem.HeatingSystemPoints
            where p.Type == HeatingSystemPointType.Temperature
            select p.TypeValue).ToArray();


        foreach (var point in frequencyParameter)
        {
            if (heatingSystemFrequencies.Contains(point)) continue;
            return false;
        }


        foreach (var point in temperatureParameter)
        {
            if (heatingSystemTemperature.Contains(point)) continue;
            return false;
        }

        return true;
    }
}
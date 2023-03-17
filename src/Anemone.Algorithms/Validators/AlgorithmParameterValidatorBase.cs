using Anemone.Algorithms.Models;
using FluentValidation;

namespace Anemone.Algorithms.Validators;

public class AlgorithmParameterValidatorBase<T> : AbstractValidator<T> where T : MatchingParameterBase
{
    public AlgorithmParameterValidatorBase()
    {
        RuleFor(x => x.FrequencyMin).NotNull();
        RuleFor(x => x.FrequencyMax).NotNull();
        RuleFor(x => x.FrequencyStep).NotNull();
        
        RuleFor(x => x.TemperatureMin).NotNull();
        RuleFor(x => x.TemperatureMax).NotNull();
        RuleFor(x => x.TemperatureStep).NotNull();
        
        RuleFor(x => x.TurnRatioMin).NotNull();
        RuleFor(x => x.TurnRatioMax).NotNull();
        RuleFor(x => x.TurnRatioStep).NotNull();
        
        RuleFor(x => x.Voltage).NotNull();
        RuleFor(x => x.Current).NotNull();
        RuleFor(x => x.Power).NotNull();
        
        
        RuleFor(x => x.FrequencyMin).GreaterThanOrEqualTo(0);
        RuleFor(x => x.FrequencyStep).GreaterThanOrEqualTo(0);
        RuleFor(x => x).Must(x => BeSinglePointOrRangeInAscendingOrder(x.FrequencyMin, x.FrequencyMax, x.FrequencyStep));
        
        RuleFor(x => x.TemperatureStep).GreaterThanOrEqualTo(0);
        RuleFor(x => x).Must(x => BeSinglePointOrRangeInAscendingOrder(x.TemperatureMin, x.TemperatureMax, x.TemperatureStep));

        RuleFor(x => x.TurnRatioMin).GreaterThan(0);
        RuleFor(x => x.TurnRatioStep).GreaterThanOrEqualTo(0);
        RuleFor(x => x).Must(x => BeSinglePointOrRangeInAscendingOrder(x.TurnRatioMin, x.TurnRatioMax, x.TurnRatioStep));
        
        
        RuleFor(x => x.Voltage).GreaterThan(0);
        RuleFor(x => x.Current).GreaterThan(0);
        RuleFor(x => x.Power).GreaterThan(0);
    }
    
    protected static bool BeSinglePointOrRangeInAscendingOrder(double? min, double? max, double? step)
    {
        if (Equals(min, max))
            return true;

        if (min > max)
            return false;

        if (step <= 0)
            return false;

        return true;
    }
}
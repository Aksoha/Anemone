using FluentValidation;

namespace Anemone.Algorithms.Models;

public class AlgorithmValidator : AbstractValidator<LlcAlgorithmParameters>
{
    public AlgorithmValidator()
    {

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

        RuleFor(x => x.InductanceMin).GreaterThan(0);
        RuleFor(x => x.InductanceStep).GreaterThanOrEqualTo(0);
        RuleFor(x => x).Must(x => BeSinglePointOrRangeInAscendingOrder(x.InductanceMin, x.InductanceMax, x.InductanceStep));

        RuleFor(x => x.CapacitanceMin).GreaterThan(0);
        RuleFor(x => x.CapacitanceStep).GreaterThanOrEqualTo(0);
        RuleFor(x => x).Must(x => BeSinglePointOrRangeInAscendingOrder(x.CapacitanceMin, x.CapacitanceMax, x.CapacitanceStep));
    }
    
    private static bool BeSinglePointOrRangeInAscendingOrder(double min, double max, double step)
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
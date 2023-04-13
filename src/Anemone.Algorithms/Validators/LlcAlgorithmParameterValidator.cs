using Anemone.Algorithms.Models;
using FluentValidation;

namespace Anemone.Algorithms.Validators;

public class LlcAlgorithmParameterValidator : AlgorithmParameterValidatorBase<LlcMatchingParameters>
{
    public LlcAlgorithmParameterValidator()
    {
        RuleFor(x => x.InductanceMin).NotNull();
        RuleFor(x => x.InductanceMax).NotNull();
        RuleFor(x => x.InductanceStep).NotNull();
        
        RuleFor(x => x.CapacitanceMin).NotNull();
        RuleFor(x => x.CapacitanceMax).NotNull();
        RuleFor(x => x.CapacitanceStep).NotNull();
        
        RuleFor(x => x.InductanceMin).GreaterThan(0);
        RuleFor(x => x.InductanceStep).GreaterThanOrEqualTo(0);
        RuleFor(x => x).Must(x => BeSinglePointOrRangeInAscendingOrder(x.InductanceMin, x.InductanceMax, x.InductanceStep));

        RuleFor(x => x.CapacitanceMin).GreaterThan(0);
        RuleFor(x => x.CapacitanceStep).GreaterThanOrEqualTo(0);
        RuleFor(x => x).Must(x => BeSinglePointOrRangeInAscendingOrder(x.CapacitanceMin, x.CapacitanceMax, x.CapacitanceStep));
    }
}
using Anemone.Algorithms.Builders;
using Anemone.Algorithms.Models;
using FluentValidation;

namespace Anemone.Algorithms.Validators;

public class
    LlcMatchingCalculatorValidator : MatchingCalculatorValidatorBase<LlcMatchingBuildArgs, LlcMatchingParameter>
{
    public LlcMatchingCalculatorValidator(IValidator<LlcMatchingParameter> validator) : base(validator)
    {
    }
}
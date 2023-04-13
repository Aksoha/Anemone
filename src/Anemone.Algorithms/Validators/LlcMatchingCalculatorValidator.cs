using Anemone.Algorithms.Builders;
using Anemone.Algorithms.Models;
using FluentValidation;

namespace Anemone.Algorithms.Validators;

public class
    LlcMatchingCalculatorValidator : MatchingCalculatorValidatorBase<LlcMatchingBuildArgs, LlcMatchingParameters>
{
    public LlcMatchingCalculatorValidator(IValidator<LlcMatchingParameters> validator) : base(validator)
    {
    }
}
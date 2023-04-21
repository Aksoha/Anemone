using Anemone.Core.EnergyMatching.Builders;
using Anemone.Core.EnergyMatching.Parameters;
using FluentValidation;

namespace Anemone.UI.Calculation.Validators;

public class
    LlcMatchingCalculatorValidator : MatchingCalculatorValidatorBase<LlcMatchingBuildArgs, LlcMatchingParameters>
{
    public LlcMatchingCalculatorValidator(IValidator<LlcMatchingParameters> validator) : base(validator)
    {
    }
}
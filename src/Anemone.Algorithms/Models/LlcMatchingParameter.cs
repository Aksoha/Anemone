
namespace Anemone.Algorithms.Models;

public class LlcMatchingParameters : MatchingParametersBase
{
    public double? InductanceMin { get; set; }
    public double? InductanceMax { get; set; }
    public double? InductanceStep { get; set; }
    
    
    public double? CapacitanceMin { get; set; }
    public double? CapacitanceMax { get; set; }
    public double? CapacitanceStep { get; set; }

    public bool VariableInductance { get; set; }
}
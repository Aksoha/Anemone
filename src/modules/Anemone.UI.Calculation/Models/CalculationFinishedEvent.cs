using Anemone.Core.EnergyMatching.Results;
using Prism.Events;

namespace Anemone.UI.Calculation.Models;

public class CalculationFinishedEvent : PubSubEvent<MatchingResultPoint[]>
{
}
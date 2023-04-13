using Prism.Events;

namespace Anemone.Algorithms.Models;

internal class CalculationFinishedEvent : PubSubEvent<MatchingResultPoint[]>
{
    
}
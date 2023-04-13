using Anemone.Core;

namespace Anemone.Algorithms.ViewModels;

public class LlcChartsViewModel : ViewModelBase
{
    public PowerMatchingChartViewModel PowerMatchingChartViewModel { get; }
    public FrequencyMatchingChartViewModel FrequencyMatchingChartViewModel { get; }
    public InductanceMatchingChartViewModel InductanceMatchingChartViewModel { get; }

    public LlcChartsViewModel(PowerMatchingChartViewModel powerMatchingChartViewModel,
        FrequencyMatchingChartViewModel frequencyMatchingChartViewModel, InductanceMatchingChartViewModel inductanceMatchingChartViewModel)
    {
        FrequencyMatchingChartViewModel = frequencyMatchingChartViewModel;
        PowerMatchingChartViewModel = powerMatchingChartViewModel;
        InductanceMatchingChartViewModel = inductanceMatchingChartViewModel;
    }
    
}
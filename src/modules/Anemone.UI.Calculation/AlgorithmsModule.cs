using Anemone.Core.EnergyMatching;
using Anemone.Core.EnergyMatching.Builders;
using Anemone.Core.EnergyMatching.Parameters;
using Anemone.Core.EnergyMatching.Results;
using Anemone.UI.Calculation.Validators;
using Anemone.UI.Calculation.ViewModels;
using Anemone.UI.Calculation.Views;
using Anemone.UI.Core;
using Anemone.UI.Core.Navigation;
using Anemone.UI.Core.Navigation.Regions;
using FluentValidation;
using Prism.Ioc;
using Prism.Modularity;

namespace Anemone.UI.Calculation;

public class AlgorithmsModule : IModule
{
    public AlgorithmsModule(INavigationManager navigationManager, IRegionCollection regionCollection)
    {
        NavigationManager = navigationManager;
        RegionCollection = regionCollection;
    }

    private INavigationManager NavigationManager { get; }
    private IRegionCollection RegionCollection { get; }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry
            .Register<IValidator<MatchingParametersBase>, AlgorithmParameterValidatorBase<MatchingParametersBase>>();
        containerRegistry.Register<IValidator<LlcMatchingParameters>, LlcAlgorithmParameterValidator>();
        containerRegistry.Register<IValidator<LlcMatchingBuildArgs>, LlcMatchingCalculatorValidator>();

        
        containerRegistry.Register<HeatingRepositoryListView>();
        
        containerRegistry.Register<IMatchingCalculator<LlcMatchingParameters, LlcMatchingResultSummary>>(x =>
            x.Resolve<ILlcMatchingCalculator>());
        
        containerRegistry.RegisterForNavigation<LlcChartsView>();
        containerRegistry.RegisterForNavigation<HeatingRepositoryListView>();
        containerRegistry.RegisterForNavigation<LlcAlgorithmView>(NavigationNames.Calculation);
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
        RegionCollection.Add(RegionNames.Sidebar, typeof(LlcAlgorithmViewModel));
        NavigationManager.Navigate(RegionNames.ContentRegion, NavigationNames.Calculation);
    }
}
using Anemone.Algorithms.Models;
using Anemone.Algorithms.Views;
using Anemone.Core;
using Anemone.DataImport.Views;
using FluentValidation;
using Prism.Ioc;
using Prism.Modularity;

namespace Anemone.Algorithms;

public class AlgorithmsModule : IModule
{
    private INavigationRegistrations NavigationRegistrations { get; }
    private IApplicationCommands ApplicationCommands { get; }
    
    public AlgorithmsModule(INavigationRegistrations navigationRegistrations, IApplicationCommands applicationCommands)
    {
        NavigationRegistrations = navigationRegistrations;
        ApplicationCommands = applicationCommands;
    }
    
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.Register<IValidator<LlcAlgorithmParameters>, AlgorithmValidator>();
        containerRegistry.Register<HeatingRepositoryListView>();
        containerRegistry.RegisterForNavigation<LlcAlgorithmView>();
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
    }
}
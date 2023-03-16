using Anemone.Algorithms.Models;
using Anemone.Algorithms.Views;
using Anemone.Core;
using FluentValidation;
using MatchingAlgorithm.Llc;
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
        containerRegistry.Register<ILlcMatchingBuilder, LlcMatchingBuilder>();
        containerRegistry.Register<IMatchingBuilder<LlcMatching, LlcAlgorithmParameters>>(x => x.Resolve<ILlcMatchingBuilder>());
        containerRegistry.Register<HeatingRepositoryListView>();
        
        containerRegistry.RegisterForNavigation<LlcAlgorithmView>();
        NavigationRegistrations.Register(new NavigationPanelItem
        {
            Header = "Llc",
            NavigationPath = nameof(LlcAlgorithmView),
            Icon = PackIconKind.HeatingSystemMatching
        });
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
        ApplicationCommands.NavigateCommand.Execute(nameof(LlcAlgorithmView));
    }
}
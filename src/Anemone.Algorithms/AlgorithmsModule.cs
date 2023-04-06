using Anemone.Algorithms.Builders;
using Anemone.Algorithms.Matching;
using Anemone.Algorithms.Models;
using Anemone.Algorithms.Report;
using Anemone.Algorithms.Validators;
using Anemone.Algorithms.Views;
using Anemone.Core;
using FluentValidation;
using MatchingAlgorithm.Llc;
using Prism.Ioc;
using Prism.Modularity;
using LlcMatchingParameter = Anemone.Algorithms.Models.LlcMatchingParameter;
using LlcMatchingResult = Anemone.Algorithms.Models.LlcMatchingResult;

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
        containerRegistry.Register<IValidator<MatchingParameterBase>, AlgorithmParameterValidatorBase<MatchingParameterBase>>();
        containerRegistry.Register<IValidator<LlcMatchingParameter>, LlcAlgorithmParameterValidator>();
        containerRegistry.Register<IValidator<LlcMatchingBuildArgs>, LlcMatchingCalculatorValidator>();
        
        
        containerRegistry.Register<ILlcMatchingBuilder, LlcMatchingBuilder>();
        containerRegistry.Register<IMatchingBuilder<LlcMatching, LlcMatchingBuildArgs>>(x => x.Resolve<ILlcMatchingBuilder>());
        containerRegistry.Register<HeatingRepositoryListView>();

        containerRegistry.Register<ILlcMatchingCalculator, LlcMatchingCalculator>();
        containerRegistry.Register<IMatchingCalculator<LlcMatchingParameter, LlcMatchingResult>>(x =>
            x.Resolve<ILlcMatchingCalculator>());

        containerRegistry.Register<IDataExporter, DataExporter>();
        containerRegistry.Register<IReportGenerator, ReportGenerator>();
        
        
        containerRegistry.RegisterForNavigation<HeatingRepositoryListView>();
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
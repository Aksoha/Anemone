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

namespace Anemone.Algorithms;

public class AlgorithmsModule : IModule
{
    public AlgorithmsModule(INavigationRegistrations navigationRegistrations, IApplicationCommands applicationCommands)
    {
        NavigationRegistrations = navigationRegistrations;
        ApplicationCommands = applicationCommands;
    }

    private INavigationRegistrations NavigationRegistrations { get; }
    private IApplicationCommands ApplicationCommands { get; }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry
            .Register<IValidator<MatchingParametersBase>, AlgorithmParameterValidatorBase<MatchingParametersBase>>();
        containerRegistry.Register<IValidator<LlcMatchingParameters>, LlcAlgorithmParameterValidator>();
        containerRegistry.Register<IValidator<LlcMatchingBuildArgs>, LlcMatchingCalculatorValidator>();


        containerRegistry.Register<ILlcMatchingBuilder, LlcMatchingBuilder>();
        containerRegistry.Register<IMatchingBuilder<LlcMatching, LlcMatchingBuildArgs>>(x =>
            x.Resolve<ILlcMatchingBuilder>());
        containerRegistry.Register<HeatingRepositoryListView>();

        containerRegistry.Register<ILlcMatchingCalculator, LlcMatchingCalculator>();
        containerRegistry.Register<IMatchingCalculator<LlcMatchingParameters, LlcMatchingResultSummary>>(x =>
            x.Resolve<ILlcMatchingCalculator>());

        containerRegistry.Register<IDataExporter, DataExporter>();
        containerRegistry.Register<IReportGenerator, ReportGenerator>();

        containerRegistry.RegisterForNavigation<LlcChartsView>();
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
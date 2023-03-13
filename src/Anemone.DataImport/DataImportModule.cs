using Anemone.Core;
using Anemone.DataImport.Services;
using Anemone.DataImport.ViewModels;
using Anemone.DataImport.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace Anemone.DataImport;

public class DataImportModule : IModule
{
    public DataImportModule(INavigationRegistrations navigationRegistrations, IApplicationCommands applicationCommands)
    {
        NavigationRegistrations = navigationRegistrations;
        ApplicationCommands = applicationCommands;
    }

    private INavigationRegistrations NavigationRegistrations { get; }
    private IApplicationCommands ApplicationCommands { get; }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.Register<ISheetFileReader, SheetFileReader>();

        NavigationRegistrations.Register(new NavigationPanelItem
        {
            Header = "Import data", NavigationPath = nameof(DataImportView),
            Icon = PackIconKind.ImportInductionHeatingData
        });
        containerRegistry.Register<DropFileViewModel>();
        containerRegistry.Register<MapColumnsViewModel>();
        containerRegistry.Register<SaveDataViewModel>();
        containerRegistry.Register<ImportDataPreviewChartView>();
        containerRegistry.Register<HeatingRepositoryListViewModel>();
        containerRegistry.RegisterForNavigation<DataImportView>();
        containerRegistry.RegisterForNavigation<SaveDataViewModel>();
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
    }
}
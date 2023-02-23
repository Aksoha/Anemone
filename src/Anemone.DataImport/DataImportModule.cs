using Anemone.Core;
using Anemone.DataImport.Services;
using Anemone.DataImport.ViewModels;
using Anemone.DataImport.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Anemone.DataImport;

public class DataImportModule : IModule
{
    private INavigationRegistrations NavigationRegistrations { get; }
    private IApplicationCommands ApplicationCommands { get; }


    public DataImportModule(INavigationRegistrations navigationRegistrations, IApplicationCommands applicationCommands)
    {
        NavigationRegistrations = navigationRegistrations;
        ApplicationCommands = applicationCommands;
    }
    
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.Register<ISheetFileReader, SheetFileReader>();
        
        NavigationRegistrations.Register(new NavigationPanelItem {Header = "Import data", NavigationPath = nameof(DataImportView), Icon = PackIconKind.ImportInductionHeatingData});
        containerRegistry.Register<DropFileViewModel>();
        containerRegistry.Register<MapColumnsViewModel>();
        containerRegistry.RegisterForNavigation<DataImportView>();
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
    }
}
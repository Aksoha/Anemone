using Anemone.UI.Core;
using Anemone.UI.Core.Navigation;
using Anemone.UI.Core.Navigation.Regions;
using Anemone.UI.DataImport.ViewModels;
using Anemone.UI.DataImport.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace Anemone.UI.DataImport;

public class DataImportModule : IModule
{
    public DataImportModule(INavigationManager navigationManager, IRegionCollection regionCollection)
    {
        NavigationManager = navigationManager;
        RegionCollection = regionCollection;
    }

    private INavigationManager NavigationManager { get; }
    private IRegionCollection RegionCollection { get; }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {

        containerRegistry.Register<DropFileViewModel>();
        containerRegistry.Register<MapColumnsViewModel>();
        containerRegistry.Register<SaveDataViewModel>();
        containerRegistry.Register<ImportDataPreviewChartView>();
        containerRegistry.RegisterForNavigation<DataImportView>();
        containerRegistry.RegisterForNavigation<SaveDataViewModel>();
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
        RegionCollection.Add(RegionNames.Sidebar, typeof(DataImportViewModel));
    }
}
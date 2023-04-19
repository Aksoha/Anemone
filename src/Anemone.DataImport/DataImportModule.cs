using Anemone.Core;
using Anemone.Core.Navigation;
using Anemone.Core.Navigation.Regions;
using Anemone.DataImport.Services;
using Anemone.DataImport.ViewModels;
using Anemone.DataImport.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace Anemone.DataImport;

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
        containerRegistry.Register<ISheetFileReader, SheetFileReader>();

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
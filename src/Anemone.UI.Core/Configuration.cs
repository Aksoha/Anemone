using Anemone.UI.Core.Dialogs;
using Anemone.UI.Core.Navigation;
using Anemone.UI.Core.Navigation.Regions;
using Anemone.UI.Core.Notifications;
using Prism.Ioc;

namespace Anemone.UI.Core;

public static class Configuration
{
    
    public static IContainerRegistry AddUi(this IContainerRegistry containerRegistry)
    {

        containerRegistry.RegisterSingleton<IToastService, ToastService>();
        containerRegistry.RegisterSingleton<INavigationManager, NavigationManager>();
        containerRegistry.RegisterSingleton<IRegionCollection, RegionCollection>();
        
        
        containerRegistry.RegisterSingleton<IOpenFileDialog, OpenFileDialog>();
        containerRegistry.RegisterSingleton<IDialogService, PrismDialogWrapper>();
        
        
        containerRegistry.RegisterDialog<TextBoxDialog, TextBoxDialogViewModel>();
        containerRegistry.RegisterDialog<ConfirmationDialog, ConfirmationDialogViewModel>();
        containerRegistry.RegisterDialogWindow<DialogWindow>();
        containerRegistry.Register<ISaveFileDialog, SaveFileDialog>();
        
        
        return containerRegistry;
    } 
}
using System.Windows;
using Anemone.Views;
using Prism.Ioc;

namespace Anemone
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        protected override Window CreateShell()
        {
            var w = Container.Resolve<ShellView>();
            return w;
        }
    }
}
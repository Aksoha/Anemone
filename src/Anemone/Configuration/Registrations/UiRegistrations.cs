using Anemone.Models;
using Anemone.UI.Core;
using Microsoft.Extensions.Logging;
using Prism.Ioc;

namespace Anemone.Configuration.Registrations;

internal static class UiRegistrations
{
    public static void Register(IContainerRegistry container, ApplicationArguments arguments,
        ILogger<RegistrationsFacade> logger)
    {
        container.AddUi();
    }
}
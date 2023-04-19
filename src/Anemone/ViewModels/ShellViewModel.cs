using Anemone.Core;
using Anemone.Settings;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;

namespace Anemone.ViewModels;

public class ShellViewModel : ViewModelBase
{
    public ShellViewModel(ShellSettings settings,
        ILogger<ShellViewModel> logger, ISnackbarMessageQueue snackbarMessageQueue)
    {
        Settings = settings;

        Logger = logger;
        SnackbarMessageQueue = snackbarMessageQueue;
    }

    public ShellSettings Settings { get; set; }
    private ILogger<ShellViewModel> Logger { get; }
    public ISnackbarMessageQueue SnackbarMessageQueue { get; set; }
}
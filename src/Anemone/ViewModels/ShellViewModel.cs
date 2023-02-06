using Anemone.Settings;

namespace Anemone.ViewModels;

public class ShellViewModel
{
    public ShellViewModel(ShellSettings settings)
    {
        Settings = settings;
    }

    public ShellSettings Settings { get; set; }
}
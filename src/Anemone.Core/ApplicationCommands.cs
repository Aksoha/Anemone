using Prism.Commands;

namespace Anemone.Core;

public class ApplicationCommands : IApplicationCommands
{
    public CompositeCommand NavigateCommand { get; } = new();
}
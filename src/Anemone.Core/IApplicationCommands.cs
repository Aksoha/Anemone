using Prism.Commands;

namespace Anemone.Core;

public interface IApplicationCommands
{
    CompositeCommand NavigateCommand { get; }
}
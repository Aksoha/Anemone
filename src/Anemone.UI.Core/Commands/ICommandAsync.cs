using System.Windows.Input;

namespace Anemone.UI.Core.Commands;

public interface ICommandAsync : ICommand
{
    Task ExecuteAsync(object? parameters);
}
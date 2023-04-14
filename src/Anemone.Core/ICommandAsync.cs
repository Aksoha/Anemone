using System.Threading.Tasks;
using System.Windows.Input;

namespace Anemone.Core;

public interface ICommandAsync : ICommand
{
    Task ExecuteAsync(object? parameters);
}
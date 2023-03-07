using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Anemone.Core;

public class ActionCommandAsync : ICommand
{
    private readonly Func<bool>? _canExecuteHandler;
    private readonly Func<Task> _executedHandler;

    public ActionCommandAsync(Func<Task> executedHandler, Func<bool>? canExecuteHandler = null)
    {
        _executedHandler = executedHandler;
        _canExecuteHandler = canExecuteHandler;
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecuteHandler == null || _canExecuteHandler();
    }

    public void Execute(object? parameter)
    {
        _executedHandler();
    }

    public event EventHandler? CanExecuteChanged;
}
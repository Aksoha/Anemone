namespace Anemone.UI.Core.Commands;

public class ActionCommandAsync : ICommandAsync
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

    public async void Execute(object? parameter)
    {
        await ExecuteAsync(parameter);
    }

    public event EventHandler? CanExecuteChanged;
    public Task ExecuteAsync(object? parameters)
    {
        return _executedHandler();
    }
}
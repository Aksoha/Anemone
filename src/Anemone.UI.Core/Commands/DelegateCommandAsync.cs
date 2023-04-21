using System.Linq.Expressions;
using Prism.Commands;

namespace Anemone.UI.Core.Commands;

public class DelegateCommandAsync : DelegateCommandBase, ICommandAsync
{
    private readonly Func<Task> _command;
    private Func<bool> _canExecuteMethod;

    public DelegateCommandAsync(Func<Task> command) : this(command, () => true)
    {
    }

    public DelegateCommandAsync(Func<Task> command, Func<bool> canExecuteMethod)
    {
        _command = command;
        _canExecuteMethod = canExecuteMethod;
    }

    public Task ExecuteAsync(object? parameter)
    {
        return _command();
    }

    protected override async void Execute(object? parameter)
    {
        await ExecuteAsync(parameter);
    }


    protected override bool CanExecute(object? parameter)
    {
        return _canExecuteMethod();
    }


    public DelegateCommandAsync ObservesProperty<T>(Expression<Func<T>> propertyExpression)
    {
        ObservesPropertyInternal(propertyExpression);
        return this;
    }

    public DelegateCommandAsync ObservesCanExecute(Expression<Func<bool>> canExecuteExpression)
    {
        _canExecuteMethod = canExecuteExpression.Compile();
        ObservesPropertyInternal(canExecuteExpression);
        return this;
    }
}
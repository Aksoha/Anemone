using System;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Core;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace Anemone.Core.ViewModels;

public class ChangeNameDialogViewModel : ViewModelBase, IDialogAware
{
    private string _message = string.Empty;
    
    public ChangeNameDialogViewModel()
    {
        CancelDialogCommand = new ActionCommand(() => CloseDialog(ButtonResult.Cancel));
        ConfirmDialogCommand =
            new DelegateCommand(() => CloseDialog(ButtonResult.OK)).ObservesCanExecute(() => CanConfirmDialog);
    }

    public ICommand CancelDialogCommand { get; }
    public ICommand ConfirmDialogCommand { get; }
    private bool CanConfirmDialog => string.IsNullOrWhiteSpace(Message) is false;

    public string Message
    {
        get => _message;
        set
        {
            if (SetProperty(ref _message, value))
                RaisePropertyChanged(nameof(CanConfirmDialog));
        }
    }

    public static string MessageParameter = nameof(Message);
    
    public string Title => "Rename";
    public event Action<IDialogResult>? RequestClose;

    public virtual bool CanCloseDialog()
    {
        return true;
    }

    public virtual void OnDialogClosed()
    {
    }

    public virtual void OnDialogOpened(IDialogParameters parameters)
    {
        Message = parameters.GetValue<string>(MessageParameter);
    }

    protected virtual void CloseDialog(ButtonResult parameter)
    {
        var query = DialogQueryBuilder.Create(MessageParameter, Message);
        RaiseRequestClose(new DialogResult(parameter, new DialogParameters(query)));
    }

    public virtual void RaiseRequestClose(IDialogResult dialogResult)
    {
        RequestClose?.Invoke(dialogResult);
    }
}
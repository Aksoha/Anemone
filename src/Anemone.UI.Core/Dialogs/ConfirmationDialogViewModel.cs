using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Core;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace Anemone.UI.Core.Dialogs;

internal class ConfirmationDialogViewModel : ViewModelBase, IDialogAware
{
    public const string TitleParameter = nameof(Title);
    public const string MessageParameter = nameof(Message);
    public const string CancelButtonTextParameter = nameof(CancelButtonText);
    public const string ConfirmButtonTextParameter = nameof(ConfirmButtonText);

    public ConfirmationDialogViewModel()
    {
        CancelDialogCommand = new ActionCommand(() => CloseDialog(ButtonResult.Cancel));
        ConfirmDialogCommand =
            new DelegateCommand(() => CloseDialog(ButtonResult.OK));
    }

    public ICommand CancelDialogCommand { get; }
    public ICommand ConfirmDialogCommand { get; }
    public string Message { get; set; } = string.Empty;
    public string CancelButtonText { get; set; } = "Cancel";
    public string ConfirmButtonText { get; set; } = "Ok";


    public string Title { get; set; } = string.Empty;


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
        Title = parameters.GetValue<string>(TitleParameter);
        Message = parameters.GetValue<string>(MessageParameter);
        CancelButtonText = parameters.GetValue<string>(CancelButtonTextParameter);
        ConfirmButtonText = parameters.GetValue<string>(ConfirmButtonTextParameter);
    }

    protected virtual void CloseDialog(ButtonResult parameter)
    {
        RaiseRequestClose(new DialogResult(parameter));
    }

    public virtual void RaiseRequestClose(IDialogResult dialogResult)
    {
        RequestClose?.Invoke(dialogResult);
    }
}
using System.Diagnostics;
using Prism.Services.Dialogs;
using PrismDialog = Prism.Services.Dialogs.IDialogService;

namespace Anemone.UI.Core.Dialogs;

internal class PrismDialogWrapper : IDialogService
{
    public PrismDialogWrapper(PrismDialog prismDialog)
    {
        PrismDialog = prismDialog;
    }

    private PrismDialog PrismDialog { get; }

    public TextBoxDialogResult ShowTextBoxDialog(string text, string title = "")
    {
        var parameter = new DialogParameters
        {
            { TextBoxDialogViewModel.MessageParameter, text },
            { TextBoxDialogViewModel.TitleParameter, title }
        };

        TextBoxDialogResult? result = null;
        PrismDialog.ShowDialog(nameof(TextBoxDialog), parameter,
            r => { result = new TextBoxDialogResult(r); });

        return result ?? throw new UnreachableException($"{nameof(result)} should be assigned before returning");
    }

    public ConfirmationDialogResult ShowConfirmationDialog(string message, string title = "",
        string cancelButtonText = "Cancel", string confirmButtonText = "Confirm")
    {
        var parameters = new DialogParameters
        {
            { ConfirmationDialogViewModel.TitleParameter, title },
            { ConfirmationDialogViewModel.MessageParameter, message },
            { ConfirmationDialogViewModel.CancelButtonTextParameter, cancelButtonText },
            { ConfirmationDialogViewModel.ConfirmButtonTextParameter, confirmButtonText }
        };

        ConfirmationDialogResult? result = null;
        PrismDialog.ShowDialog(nameof(ConfirmationDialog), parameters,
            r => { result = new ConfirmationDialogResult(r); });

        return result ?? throw new UnreachableException($"{nameof(result)} should be assigned before returning");
    }
}
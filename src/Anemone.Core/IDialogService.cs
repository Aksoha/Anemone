namespace Anemone.Core;

public interface IDialogService
{
    TextBoxDialogResult ShowTextBoxDialog(string text, string title = "");

    ConfirmationDialogResult ShowConfirmationDialog(string message, string title = "",
        string cancelButtonText = "Cancel", string confirmButtonText = "Confirm");
}
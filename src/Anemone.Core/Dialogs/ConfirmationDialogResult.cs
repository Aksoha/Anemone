using Prism.Services.Dialogs;

namespace Anemone.Core.Dialogs;

public class ConfirmationDialogResult : IDialogResult
{
    public ConfirmationDialogResult()
    {
        Parameters = new DialogParameters();
        Result = ButtonResult.None;
    }

    public ConfirmationDialogResult(IDialogResult result)
    {
        Result = result.Result;
        Parameters = result.Parameters;
    }

    public IDialogParameters Parameters { get; set; }
    public ButtonResult Result { get; set; }
}
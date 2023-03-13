using Anemone.Core.ViewModels;
using Prism.Services.Dialogs;

namespace Anemone.Core;

public class TextBoxDialogResult : IDialogResult
{
    public TextBoxDialogResult()
    {
        Parameters = new DialogParameters();
        Result = ButtonResult.None;
        Text = string.Empty;
    }

    public TextBoxDialogResult(IDialogResult result)
    {
        Result = result.Result;
        Parameters = result.Parameters;
        Text = Parameters.GetValue<string>(TextBoxDialogViewModel.MessageParameter);
    }

    public string Text { get; set; }
    public IDialogParameters Parameters { get; set; }
    public ButtonResult Result { get; set; }
}
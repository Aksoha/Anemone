using Anemone.Core.ViewModels;
using Prism.Services.Dialogs;

namespace Anemone.Core;

public class ChangeNameDialogResult : IDialogResult
{
    public IDialogParameters Parameters { get; set; }
    public ButtonResult Result { get; set; }
    public string Text { get; set; }

    public ChangeNameDialogResult()
    {
        Parameters = new DialogParameters();
        Result = ButtonResult.None;
        Text = string.Empty;

    }
    
    public ChangeNameDialogResult(IDialogResult result)
    {
        Result = result.Result;
        Parameters = result.Parameters;
        Text = Parameters.GetValue<string>(ChangeNameDialogViewModel.MessageParameter);
    }
}
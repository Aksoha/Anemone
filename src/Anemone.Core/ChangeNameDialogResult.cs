using Anemone.Core.ViewModels;
using Prism.Services.Dialogs;

namespace Anemone.Core;

public class ChangeNameDialogResult : IDialogResult
{
    public IDialogParameters Parameters { get; }
    public ButtonResult Result { get; }
    public string Text => Parameters.GetValue<string>(ChangeNameDialogViewModel.MessageParameter);
    public ChangeNameDialogResult(IDialogResult result)
    {
        Result = result.Result;
        Parameters = result.Parameters;
    }
}
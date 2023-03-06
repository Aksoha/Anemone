using System.Diagnostics;
using Anemone.Core.Components;
using Anemone.Core.ViewModels;
using Prism.Services.Dialogs;

namespace Anemone.Core;

public static class DialogServiceExtensions
{
    public static ChangeNameDialogResult ShowChangeNameDialog(this IDialogService dialogService, string message)
    {
        var parameter = DialogQueryBuilder.Create(ChangeNameDialogViewModel.MessageParameter, message);

        ChangeNameDialogResult? result = null;
        dialogService.ShowDialog(nameof(ChangeNameDialog), new DialogParameters(parameter), r =>
        {
            result = new ChangeNameDialogResult(r);
        });

        return result ?? throw new UnreachableException($"{nameof(result)} should be assigned before returning");
    }
}
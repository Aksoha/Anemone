using System.Diagnostics;
using Anemone.Core.Components;
using Anemone.Core.ViewModels;
using Prism.Services.Dialogs;
using PrismDialog = Prism.Services.Dialogs.IDialogService;

namespace Anemone.Core;

public class PrismDialogWrapper : IDialogService
{
    private PrismDialog PrismDialog { get; }

    public PrismDialogWrapper(PrismDialog prismDialog)
    {
        PrismDialog = prismDialog;
    }
    
    public ChangeNameDialogResult ShowChangeNameDialog(string name)
    {
        var parameter = DialogQueryBuilder.Create(ChangeNameDialogViewModel.MessageParameter, name);

        ChangeNameDialogResult? result = null;
        PrismDialog.ShowDialog(nameof(ChangeNameDialog), new DialogParameters(parameter), r =>
        {
            result = new ChangeNameDialogResult(r);
        });

        return result ?? throw new UnreachableException($"{nameof(result)} should be assigned before returning");
    }
}
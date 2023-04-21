using Prism.Services.Dialogs;

namespace Anemone.UI.Core.Dialogs;

internal partial class DialogWindow : IDialogWindow
{
    public DialogWindow()
    {
        InitializeComponent();
    }

    public IDialogResult Result { get; set; }
}
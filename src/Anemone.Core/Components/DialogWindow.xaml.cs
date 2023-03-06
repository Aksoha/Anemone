using Prism.Services.Dialogs;

namespace Anemone.Core.Components;

public partial class DialogWindow : IDialogWindow
{
    public DialogWindow()
    {
        InitializeComponent();
    }

    public IDialogResult Result { get; set; }
}
using System.Windows.Controls;

namespace Anemone.UI.Core.Dialogs;

internal partial class TextBoxDialog : UserControl
{
    public TextBoxDialog()
    {
        InitializeComponent();
        TextBox.Focus();
    }
}
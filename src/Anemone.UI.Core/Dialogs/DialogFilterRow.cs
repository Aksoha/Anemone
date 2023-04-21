using System.Text;
using Anemone.Core.Common.ValueObjects;

namespace Anemone.UI.Core.Dialogs;

public class DialogFilterRow
{
    public DialogFilterRow(string filterName, IEnumerable<Func<FileExtension>> extensions)
    {
        _filterName = filterName;

        foreach (var extension in extensions)
        {
            _extensions.Add(extension.Invoke());
        }
        
    }

    private readonly string _filterName;
    private readonly List<FileExtension> _extensions = new();
    

    public static implicit operator string(DialogFilterRow filter)
    {
        return filter.FormatAsString();
    }

    private string FormatAsString()
    {
        if (!_extensions.Any())
            throw new DialogExtensionCountException();
        
        
        var builder = new StringBuilder();
        builder.Append(_filterName);
        builder.Append('|');

        foreach (var extension in _extensions)
        {
            builder.Append($"*{extension};");
        }
        builder.Length--;

        return builder.ToString();
    }
    public override string ToString() => this;
}
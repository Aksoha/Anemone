using System.Collections.Generic;
using System.Text;

namespace Anemone.Core.Dialogs;

public class DialogFilterCollection
{
    private readonly List<DialogFilterRow> _filters = new();

    public void AddFilterRow(DialogFilterRow row)
    {
        _filters.Add(row);
    }

    public static implicit operator string(DialogFilterCollection filterCollection)
    {
        return filterCollection.FormatAsString();
    }
    
    private string FormatAsString()
    {
        switch (_filters.Count)
        {
            case 0:
                throw new DialogFilterCountException();
            case 1:
                return _filters[0];
        }

        var builder = new StringBuilder();
        foreach (var filter in _filters)
        {
            builder.Append($"{filter}|");
        }
        builder.Length--;

        return builder.ToString();

    }
}
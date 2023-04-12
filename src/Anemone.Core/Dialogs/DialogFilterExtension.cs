namespace Anemone.Core.Dialogs;

public class DialogFilterExtension
{
    private readonly string _extension;

    private DialogFilterExtension(string extension)
    {
        _extension = extension;
    }

    public static DialogFilterExtension Csv()
    {
        return new DialogFilterExtension(".csv");
    }
    public static DialogFilterExtension Xls()
    {
        return new DialogFilterExtension(".xls");
    }

    public static DialogFilterExtension Xlsx()
    {
        return new DialogFilterExtension(".xlsx");
    }

    public static DialogFilterExtension All()
    {
        return new DialogFilterExtension(".*");
    }

    public static implicit operator string(DialogFilterExtension? filter) => filter._extension;
    public override string ToString() => this;
    public override int GetHashCode() => _extension.GetHashCode();
}
namespace Anemone.Core.Common.ValueObjects;

public class FileExtension
{
    private readonly string _extension;

    private FileExtension(string extension)
    {
        _extension = extension;
    }

    public static FileExtension Csv()
    {
        return new FileExtension(".csv");
    }
    public static FileExtension Xls()
    {
        return new FileExtension(".xls");
    }

    public static FileExtension Xlsx()
    {
        return new FileExtension(".xlsx");
    }

    public static FileExtension All()
    {
        return new FileExtension(".*");
    }

    public static implicit operator string(FileExtension filter) => filter._extension;
    public override string ToString() => this;
    public override int GetHashCode() => _extension.GetHashCode();
}
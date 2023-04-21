namespace Anemone.Core.Export;

public class ExportFileTypeNotSupportedException : ExportException
{
    public ExportFileTypeNotSupportedException(string extension) : base(FormatException(extension))
    {
        NotSupportedExtension = extension;
    }

    public string? FilePath { get; init; }
    public string NotSupportedExtension { get; }
    public string[]? SupportedExtensions { get; init; }

    private static string FormatException(string extension)
    {
        return $"file with extension \"{extension}\" is currently not supported";
    }
}
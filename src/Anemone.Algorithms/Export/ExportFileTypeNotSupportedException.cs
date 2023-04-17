namespace Anemone.Algorithms.Export;

public class ExportFileTypeNotSupportedException : ExportException
{
    public string? FilePath { get; init; }
    public string NotSupportedExtension { get; }
    public string[]? SupportedExtensions { get; init; }
    public ExportFileTypeNotSupportedException(string extension) : base(FormatException(extension))
    {
        NotSupportedExtension = extension;
    }

    private static string FormatException(string extension)
    {
        return $"file with extension \"{extension}\" is currently not supported";
    }
    
}
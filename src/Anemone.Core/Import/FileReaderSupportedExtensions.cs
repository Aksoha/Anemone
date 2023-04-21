using Anemone.Core.Common.ValueObjects;

namespace Anemone.Core.Import;

public class FileReaderSupportedExtensions
{
    public static readonly string[] Extensions =
    {
        FileExtension.Csv(),
        FileExtension.Xls(),
        FileExtension.Xlsx(),
    };
}
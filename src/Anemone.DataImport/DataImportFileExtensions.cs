using Anemone.Core.Dialogs;

namespace Anemone.DataImport;

public class DataImportFileExtensions
{
    public static readonly string[] SupportedExtensions =
    {
        DialogFilterExtension.Csv(),
        DialogFilterExtension.Xls(),
        DialogFilterExtension.Xlsx(),
    };
}
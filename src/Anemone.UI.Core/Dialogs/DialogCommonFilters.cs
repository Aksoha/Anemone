using Anemone.Core.Common.ValueObjects;

namespace Anemone.UI.Core.Dialogs;

public static class DialogCommonFilters
{
    public static readonly DialogFilterRow AllFiles;
    public static readonly DialogFilterRow CsvFiles;
    public static readonly DialogFilterRow SheetFiles;
    

    static DialogCommonFilters()
    {
        AllFiles = new DialogFilterRow("All files", new []{FileExtension.All});
        CsvFiles = new DialogFilterRow("Csv files", new []{FileExtension.Csv});
        SheetFiles = new DialogFilterRow("Sheet files", new []
        {
            FileExtension.Csv,
            FileExtension.Xls,
            FileExtension.Xlsx
        });
    }
}
namespace Anemone.Core.Dialogs;

public static class DialogCommonFilters
{
    public static readonly DialogFilterRow AllFiles;
    public static readonly DialogFilterRow CsvFiles;
    public static readonly DialogFilterRow SheetFiles;
    

    static DialogCommonFilters()
    {
        AllFiles = new DialogFilterRow("All files", new []{DialogFilterExtension.All});
        CsvFiles = new DialogFilterRow("Csv files", new []{DialogFilterExtension.Csv});
        SheetFiles = new DialogFilterRow("Sheet files", new []
        {
            DialogFilterExtension.Csv,
            DialogFilterExtension.Xls,
            DialogFilterExtension.Xlsx
        });
    }
}
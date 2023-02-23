namespace Anemone.DataImport;

public class DataImportFileExtensions
{
    public static readonly string[] SupportedExtensions = { ".xls", ".xlsx", ".csv" };
    public static readonly string Filters = "sheet files|*" + string.Join(";*", SupportedExtensions);
}
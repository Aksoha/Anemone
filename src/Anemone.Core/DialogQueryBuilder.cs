namespace Anemone.Core;

public static class DialogQueryBuilder
{
    public static string Create(string parameter, string value)
    {
        return $"{parameter}={value}";
    }
}
using System;
using System.Text;

namespace Anemone.Repository;

public class RepositoryReadException : RepositoryException
{
    public RepositoryReadException(string database, string? reason = null) : base(FormatMessage(database, reason))
    {
    }

    public RepositoryReadException(string database, Exception? innerException, string? reason = null) : base(
        FormatMessage(database, reason),
        innerException)
    {
    }

    private static string FormatMessage(string database, string? reason = null)
    {
        var builder = new StringBuilder();
        builder.Append($"An error has occured when attempting to load data from the {database}");

        if (reason is not null)
            builder.Append($" reason: {reason}");

        return builder.ToString();
    }
}
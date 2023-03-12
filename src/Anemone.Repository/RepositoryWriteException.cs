using System;
using System.Text;

namespace Anemone.Repository;

public class RepositoryWriteException : RepositoryException
{
    public RepositoryWriteException(string database, string? reason = null) : base(FormatMessage(database, reason))
    {
    }

    public RepositoryWriteException(string database, Exception? innerException, string? reason = null) : base(
        FormatMessage(database, reason),
        innerException)
    {
    }

    private static string FormatMessage(string database, string? reason = null)
    {
        var builder = new StringBuilder();
        builder.Append($"An error has occured when attempting to save data to the {database}");

        if (reason is not null)
            builder.Append($" reason: {reason}");

        return builder.ToString();
    }
}
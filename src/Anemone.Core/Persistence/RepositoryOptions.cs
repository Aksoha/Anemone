using System.Diagnostics.CodeAnalysis;

namespace Anemone.Core.Persistence;

/// <summary>
///     Options for <see cref="IRepository{T}" />.
/// </summary>
public class RepositoryOptions
{
    [SetsRequiredMembers]
    public RepositoryOptions(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public RepositoryOptions()
    {
    }

    /// <summary>
    ///     Connection string to the database.
    /// </summary>
    public required string ConnectionString { get; init; }
}
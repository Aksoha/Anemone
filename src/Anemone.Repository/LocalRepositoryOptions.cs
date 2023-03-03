namespace Anemone.Repository;

/// <summary>
///     Options for <see cref="LocalRepository" />.
/// </summary>
public class LocalRepositoryOptions
{
    /// <summary>
    ///     directory in which all data is stored.
    /// </summary>
    public required string WorkingDir { get; init; }
}
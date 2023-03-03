namespace Anemone.Repository;

public class LocalRepository
{
    protected LocalRepository(LocalRepositoryOptions localRepositoryOptions)
    {
        LocalRepositoryOptions = localRepositoryOptions;
    }

    protected LocalRepositoryOptions LocalRepositoryOptions { get; }
}
using System;

namespace Anemone.Repository;

public class RepositoryException : Exception
{
    protected RepositoryException()
    {
    }

    protected RepositoryException(string? message) : base(message)
    {
    }

    protected RepositoryException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
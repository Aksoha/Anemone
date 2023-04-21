using System;

namespace Anemone.Core.Persistence;

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
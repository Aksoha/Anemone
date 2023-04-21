using System.Collections.Generic;
using System.Threading.Tasks;
using Anemone.Core.Common.Entities;

namespace Anemone.Core.Persistence;

public interface IRepository<T> where T : IDbEntity
{
    Task<bool> Exists(int id);

    /// <summary>
    ///     Creates new entry in the database for <see cref="data" />.
    /// </summary>
    /// <param name="data">The data to create.</param>
    Task Create(T data);

    /// <summary>
    ///     Retrieves data associated with <paramref name="id" />.
    /// </summary>
    /// <param name="id">Id of the object.</param>
    Task<T?> Get(int id);

    /// <summary>
    ///     Retrieve all data.
    /// </summary>
    Task<IEnumerable<T>> GetAll();

    /// <summary>
    ///     Updates the data.
    /// </summary>
    /// <param name="data">The data to update.</param>
    Task Update(T data);

    /// <summary>
    ///     Deletes <paramref name="data" />.
    /// </summary>
    /// <param name="data">Data to delete.</param>
    Task Delete(T data);
}
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Anemone.Repository;

public interface IRepository<T> where T : class
{
    Task Create(T data, string? id = null);
    Task<T?> Get(string id);
    Task<IEnumerable<T>> GetAll();
    Task Update(string id, T data);
    Task Delete(T data);
}
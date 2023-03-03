using System.Collections.Generic;
using System.Threading.Tasks;

namespace Anemone.Repository.HeatingSystem;

public interface IHeatingSystemRepository : IRepository<PersistenceHeatingSystemModel>
{
    Task<IEnumerable<string>> GetAllNames();
}
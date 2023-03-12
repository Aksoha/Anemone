using System.Collections.Generic;
using System.Threading.Tasks;

namespace Anemone.Repository.HeatingSystemData;

public interface IHeatingSystemRepository : IRepository<HeatingSystem>
{
    Task<IEnumerable<HeatingSystemName>> GetAllNames();
}
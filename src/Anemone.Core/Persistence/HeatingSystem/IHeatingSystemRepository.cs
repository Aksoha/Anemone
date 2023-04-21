using System.Collections.Generic;
using System.Threading.Tasks;
using Anemone.Core.Common.Entities;

namespace Anemone.Core.Persistence.HeatingSystem;

public interface IHeatingSystemRepository : IRepository<Common.Entities.HeatingSystem>
{
    Task<IEnumerable<HeatingSystemName>> GetAllNames();
}
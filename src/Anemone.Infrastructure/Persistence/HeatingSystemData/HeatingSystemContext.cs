using Microsoft.EntityFrameworkCore;

namespace Anemone.Infrastructure.Persistence.HeatingSystemData;

internal class HeatingSystemContext : DbContext
{
    public HeatingSystemContext(DbContextOptions<HeatingSystemContext> options) : base(options)
    {
    }

    public DbSet<Core.Common.Entities.HeatingSystem> HeatingSystem { get; set; }
}
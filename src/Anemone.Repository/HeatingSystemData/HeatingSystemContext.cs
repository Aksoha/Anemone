using Microsoft.EntityFrameworkCore;

namespace Anemone.Repository.HeatingSystemData;

public class HeatingSystemContext : DbContext
{
    public HeatingSystemContext(DbContextOptions<HeatingSystemContext> options) : base(options)
    {
    }

    public DbSet<HeatingSystem> HeatingSystem { get; set; }
}
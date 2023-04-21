using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Anemone.Infrastructure.Persistence.HeatingSystemData;

[ExcludeFromCodeCoverage(Justification = "design time instance for creating migrations in EF")]
internal class HeatingSystemContextFactory : IDesignTimeDbContextFactory<HeatingSystemContext>
{
    public HeatingSystemContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HeatingSystemContext>();
        optionsBuilder.UseSqlite("Data Source=hs.db");

        return new HeatingSystemContext(optionsBuilder.Options);
    }
}
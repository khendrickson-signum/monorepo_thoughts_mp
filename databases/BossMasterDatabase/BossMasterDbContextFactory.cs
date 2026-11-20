using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BossMasterDatabase;

public sealed class BossMasterDbContextFactory : IDesignTimeDbContextFactory<BossMasterDbContext>
{
    public BossMasterDbContext CreateDbContext(string[] args)
    {
        DbContextOptions<BossMasterDbContext> options = new DbContextOptionsBuilder<BossMasterDbContext>()
            .UseSqlServer("Server=placeholder;Database=BossMaster;User Id=placeholder;Password=placeholder;")
            .Options;

        return new BossMasterDbContext(options);
    }
}

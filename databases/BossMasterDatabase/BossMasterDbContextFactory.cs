using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BossMasterDatabase;

public sealed class BossMasterDbContextFactory : IDesignTimeDbContextFactory<BossMasterDbContext>
{
    public BossMasterDbContext CreateDbContext(string[] args)
    {
        string? configuredConnection = Environment.GetEnvironmentVariable("EF_CONNECTION");
        string connectionString = string.IsNullOrWhiteSpace(configuredConnection)
            ? "Server=placeholder;Database=BossMaster;User Id=placeholder;Password=placeholder;"
            : configuredConnection;
        DbContextOptions<BossMasterDbContext> options = new DbContextOptionsBuilder<BossMasterDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new BossMasterDbContext(options);
    }
}

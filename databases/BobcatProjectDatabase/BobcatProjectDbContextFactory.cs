using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BobcatProjectDatabase;

public sealed class BobcatProjectDbContextFactory : IDesignTimeDbContextFactory<BobcatProjectDbContext>
{
    public BobcatProjectDbContext CreateDbContext(string[] args)
    {
        string? configuredConnection = Environment.GetEnvironmentVariable("EF_CONNECTION");
        string connectionString = string.IsNullOrWhiteSpace(configuredConnection)
            ? "Server=placeholder;Database=BobcatProject;User Id=placeholder;Password=placeholder;"
            : configuredConnection;
        DbContextOptions<BobcatProjectDbContext> options = new DbContextOptionsBuilder<BobcatProjectDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new BobcatProjectDbContext(options);
    }
}

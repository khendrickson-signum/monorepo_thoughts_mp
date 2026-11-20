using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IxomProjectDatabase;

public sealed class IxomProjectDbContextFactory : IDesignTimeDbContextFactory<IxomProjectDbContext>
{
    public IxomProjectDbContext CreateDbContext(string[] args)
    {
        string? configuredConnection = Environment.GetEnvironmentVariable("EF_CONNECTION");
        string connectionString = string.IsNullOrWhiteSpace(configuredConnection)
            ? "Server=placeholder;Database=IxomProject;User Id=placeholder;Password=placeholder;"
            : configuredConnection;
        DbContextOptions<IxomProjectDbContext> options = new DbContextOptionsBuilder<IxomProjectDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new IxomProjectDbContext(options);
    }
}

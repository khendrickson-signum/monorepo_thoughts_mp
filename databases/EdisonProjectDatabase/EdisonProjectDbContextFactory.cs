using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EdisonProjectDatabase;

public sealed class EdisonProjectDbContextFactory : IDesignTimeDbContextFactory<EdisonProjectDbContext>
{
    public EdisonProjectDbContext CreateDbContext(string[] args)
    {
        string? configuredConnection = Environment.GetEnvironmentVariable("EF_CONNECTION");
        string connectionString = string.IsNullOrWhiteSpace(configuredConnection)
            ? "Server=placeholder;Database=EdisonProject;User Id=placeholder;Password=placeholder;"
            : configuredConnection;
        DbContextOptions<EdisonProjectDbContext> options = new DbContextOptionsBuilder<EdisonProjectDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new EdisonProjectDbContext(options);
    }
}

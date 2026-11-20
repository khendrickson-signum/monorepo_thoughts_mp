using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BobcatProjectDatabase;

public sealed class BobcatProjectDbContextFactory : IDesignTimeDbContextFactory<BobcatProjectDbContext>
{
    public BobcatProjectDbContext CreateDbContext(string[] args)
    {
        DbContextOptions<BobcatProjectDbContext> options = new DbContextOptionsBuilder<BobcatProjectDbContext>()
            .UseSqlServer("Server=placeholder;Database=BobcatProject;User Id=placeholder;Password=placeholder;")
            .Options;

        return new BobcatProjectDbContext(options);
    }
}

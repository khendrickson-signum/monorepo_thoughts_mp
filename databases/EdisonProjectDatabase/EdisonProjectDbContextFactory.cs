using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EdisonProjectDatabase;

public sealed class EdisonProjectDbContextFactory : IDesignTimeDbContextFactory<EdisonProjectDbContext>
{
    public EdisonProjectDbContext CreateDbContext(string[] args)
    {
        DbContextOptions<EdisonProjectDbContext> options = new DbContextOptionsBuilder<EdisonProjectDbContext>()
            .UseSqlServer("Server=placeholder;Database=EdisonProject;User Id=placeholder;Password=placeholder;")
            .Options;

        return new EdisonProjectDbContext(options);
    }
}

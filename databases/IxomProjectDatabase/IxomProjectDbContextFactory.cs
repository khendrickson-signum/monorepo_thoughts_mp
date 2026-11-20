using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IxomProjectDatabase;

public sealed class IxomProjectDbContextFactory : IDesignTimeDbContextFactory<IxomProjectDbContext>
{
    public IxomProjectDbContext CreateDbContext(string[] args)
    {
        DbContextOptions<IxomProjectDbContext> options = new DbContextOptionsBuilder<IxomProjectDbContext>()
            .UseSqlServer("Server=placeholder;Database=IxomProject;User Id=placeholder;Password=placeholder;")
            .Options;

        return new IxomProjectDbContext(options);
    }
}

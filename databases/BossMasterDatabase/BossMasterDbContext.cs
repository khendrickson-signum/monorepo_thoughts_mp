using Microsoft.EntityFrameworkCore;

namespace BossMasterDatabase;

public class BossMasterDbContext : DbContext
{
    public BossMasterDbContext(DbContextOptions<BossMasterDbContext> options)
        : base(options)
    {
    }

    public DbSet<AllSignumDevicesAcrossAllCustomers> AllSignumDevicesAcrossAllCustomers => Set<AllSignumDevicesAcrossAllCustomers>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AllSignumDevicesAcrossAllCustomers>().HasData(BossMasterSeed.Devices);
    }
}

using Microsoft.EntityFrameworkCore;

namespace BobcatProjectDatabase;

public class BobcatProjectDbContext : DbContext
{
    public BobcatProjectDbContext(DbContextOptions<BobcatProjectDbContext> options)
        : base(options)
    {
    }

    public DbSet<DevicesForThisCustomer> DevicesForThisCustomer => Set<DevicesForThisCustomer>();

    public DbSet<MachineConfigsThisCustomerSupports> MachineConfigsThisCustomerSupports => Set<MachineConfigsThisCustomerSupports>();

    public DbSet<MachineTypesForThisCustomer> MachineTypesForThisCustomer => Set<MachineTypesForThisCustomer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DevicesForThisCustomer>().HasData(BobcatProjectSeed.Devices);
        modelBuilder.Entity<MachineTypesForThisCustomer>().HasData(BobcatProjectSeed.MachineTypes);
        modelBuilder.Entity<MachineConfigsThisCustomerSupports>().HasData(BobcatProjectSeed.MachineConfigs);
    }
}

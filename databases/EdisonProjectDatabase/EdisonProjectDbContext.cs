using Microsoft.EntityFrameworkCore;

namespace EdisonProjectDatabase;

public class EdisonProjectDbContext : DbContext
{
    public EdisonProjectDbContext(DbContextOptions<EdisonProjectDbContext> options)
        : base(options)
    {
    }

    public DbSet<DevicesForThisCustomer> DevicesForThisCustomer => Set<DevicesForThisCustomer>();

    public DbSet<MachineConfigsThisCustomerSupports> MachineConfigsThisCustomerSupports => Set<MachineConfigsThisCustomerSupports>();

    public DbSet<MachineTypesForThisCustomer> MachineTypesForThisCustomer => Set<MachineTypesForThisCustomer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DevicesForThisCustomer>().HasData(EdisonProjectSeed.Devices);
        modelBuilder.Entity<MachineTypesForThisCustomer>().HasData(EdisonProjectSeed.MachineTypes);
        modelBuilder.Entity<MachineConfigsThisCustomerSupports>().HasData(EdisonProjectSeed.MachineConfigs);
    }
}

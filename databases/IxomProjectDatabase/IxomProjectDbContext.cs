using Microsoft.EntityFrameworkCore;

namespace IxomProjectDatabase;

public class IxomProjectDbContext : DbContext
{
    public IxomProjectDbContext(DbContextOptions<IxomProjectDbContext> options)
        : base(options)
    {
    }

    public DbSet<DevicesForThisCustomer> DevicesForThisCustomer => Set<DevicesForThisCustomer>();

    public DbSet<MachineConfigsThisCustomerSupports> MachineConfigsThisCustomerSupports => Set<MachineConfigsThisCustomerSupports>();

    public DbSet<MachineTypesForThisCustomer> MachineTypesForThisCustomer => Set<MachineTypesForThisCustomer>();
}

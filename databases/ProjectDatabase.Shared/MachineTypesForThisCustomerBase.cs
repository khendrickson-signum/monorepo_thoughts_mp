namespace ProjectDatabase.Shared;

// Important: only columns guaranteed to exist in all project databases should exist in this base class.
public abstract class MachineTypesForThisCustomerBase : IMachineTypesForThisCustomer
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
}

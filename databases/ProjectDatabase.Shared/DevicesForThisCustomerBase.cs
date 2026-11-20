namespace ProjectDatabase.Shared;

// Important: only columns guaranteed to exist in all project databases should exist in this base class.
public abstract class DevicesForThisCustomerBase : IDevicesForThisCustomer
{
    public Guid Id { get; set; }

    public Guid SignumGuid { get; set; }

    public bool? IsActive { get; set; } = false;

    public string? Name { get; set; }
}

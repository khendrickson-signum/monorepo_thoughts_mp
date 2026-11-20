namespace ProjectDatabase.Shared;

// Important: only columns guaranteed to exist in all project databases should exist in this base class.
public abstract class MachineConfigsThisCustomerSupportsBase : IMachineConfigsThisCustomerSupports
{
    public Guid Id { get; set; }

    public Guid DeviceId { get; set; }

    public Guid MachineTypeId { get; set; }

    public string? Name { get; set; }

    public string? SerialNumber { get; set; }
}

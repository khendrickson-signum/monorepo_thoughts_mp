namespace ProjectDatabase.Shared;

public interface IMachineConfigsThisCustomerSupports
{
    Guid Id { get; set; }

    Guid DeviceId { get; set; }

    Guid MachineTypeId { get; set; }

    string? Name { get; set; }

    string? SerialNumber { get; set; }
}

namespace EdisonProjectDatabase;

public class MachineConfigsThisCustomerSupports
{
    public Guid Id { get; set; }

    public Guid DeviceId { get; set; }

    public Guid MachineTypeId { get; set; }

    public string? Name { get; set; }

    public string? SerialNumber { get; set; }
}

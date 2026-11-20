namespace BossMasterDatabase;

public partial class AllSignumDevicesAcrossAllCustomers
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public string? Name { get; set; }

    public bool? IsActive { get; set; }

    public string SerialNumber { get; set; } = null!;
}

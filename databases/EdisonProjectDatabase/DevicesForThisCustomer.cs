namespace EdisonProjectDatabase;

public class DevicesForThisCustomer
{
    public Guid Id { get; set; }

    public Guid SignumGuid { get; set; }

    public bool? IsActive { get; set; } = false;

    public string? Name { get; set; }
}

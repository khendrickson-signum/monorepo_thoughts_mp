namespace ProjectDatabase.Shared;

public interface IDevicesForThisCustomer
{
    Guid Id { get; set; }

    Guid SignumGuid { get; set; }

    bool? IsActive { get; set; }

    string? Name { get; set; }
}

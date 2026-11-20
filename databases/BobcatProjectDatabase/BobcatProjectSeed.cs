namespace BobcatProjectDatabase;

public static class BobcatProjectSeed
{
    public static readonly Guid Device1Id = new Guid("34132d13-a970-4340-8e10-ed260ada6498");

    public static readonly Guid Device2Id = new Guid("c56ee274-baa2-4122-990b-8eb8655c5917");

    public static readonly Guid LoaderTypeId = new Guid("192d2446-2643-4ac9-8038-1dc34f61dc7c");

    public static readonly Guid ExcavatorTypeId = new Guid("33e58b4c-8a8d-4eb4-bd90-3cbcb8d891e3");

    public static readonly Guid LoaderConfigId = new Guid("789ae17b-a7fa-4c95-91a8-3b0ed4c60239");

    public static readonly Guid ExcavatorConfigId = new Guid("e772871c-ada9-4f50-b9c6-84ab793222df");

    public static IReadOnlyList<DevicesForThisCustomer> Devices { get; } =
    [
        new DevicesForThisCustomer
        {
            Id = Device1Id,
            SignumGuid = new Guid("5962a011-138c-4e50-9d22-7dd0bb6501be"),
            IsActive = true,
            Name = "Bobcat #1",
        },
        new DevicesForThisCustomer
        {
            Id = Device2Id,
            SignumGuid = new Guid("c2165796-8976-4e4b-9719-958127a17518"),
            IsActive = true,
            Name = "Bobcat #2",
        },
    ];

    public static IReadOnlyList<MachineTypesForThisCustomer> MachineTypes { get; } =
    [
        new MachineTypesForThisCustomer
        {
            Id = LoaderTypeId,
            Name = "Loader",
        },
        new MachineTypesForThisCustomer
        {
            Id = ExcavatorTypeId,
            Name = "Excavator",
        },
    ];

    public static IReadOnlyList<MachineConfigsThisCustomerSupports> MachineConfigs { get; } =
    [
        new MachineConfigsThisCustomerSupports
        {
            Id = LoaderConfigId,
            DeviceId = Device1Id,
            MachineTypeId = LoaderTypeId,
            Name = "Bobcat Loader",
        },
        new MachineConfigsThisCustomerSupports
        {
            Id = ExcavatorConfigId,
            DeviceId = Device2Id,
            MachineTypeId = ExcavatorTypeId,
            Name = "Bobcat Excavator",
        },
    ];
}

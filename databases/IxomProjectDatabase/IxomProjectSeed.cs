namespace IxomProjectDatabase;

public static class IxomProjectSeed
{
    public static readonly Guid Device1Id = new Guid("2089940c-7c9c-47a5-a2d5-82221f5d3b9b");

    public static readonly Guid Device2Id = new Guid("571a9ea5-7fb0-4190-8639-2878d2dfac56");

    public static readonly Guid SolarBeeTypeId = new Guid("efa04a71-1751-48e0-8d55-b8c46c0acf0f");

    public static readonly Guid GsMixerTypeId = new Guid("c9dd93f0-36c4-42dd-b3ed-82d37ce23655");

    public static readonly Guid SolarBeeConfigId = new Guid("f0bec2f2-46e8-480a-9705-d80bf421d857");

    public static readonly Guid GsMixerConfigId = new Guid("cb6358cd-ef64-4597-bbcb-70513d977a68");

    public static IReadOnlyList<DevicesForThisCustomer> Devices { get; } =
    [
        new DevicesForThisCustomer
        {
            Id = Device1Id,
            SignumGuid = new Guid("1e5fc019-2561-471d-8622-357bd8675d9c"),
            IsActive = true,
            Name = "Ixom #1",
        },
        new DevicesForThisCustomer
        {
            Id = Device2Id,
            SignumGuid = new Guid("d1e12a24-a49e-41de-b4b6-0c87b67521ca"),
            IsActive = true,
            Name = "Ixom #2",
        },
    ];

    public static IReadOnlyList<MachineTypesForThisCustomer> MachineTypes { get; } =
    [
        new MachineTypesForThisCustomer
        {
            Id = SolarBeeTypeId,
            Name = "SolarBee",
        },
        new MachineTypesForThisCustomer
        {
            Id = GsMixerTypeId,
            Name = "GS Mixer",
        },
    ];

    public static IReadOnlyList<MachineConfigsThisCustomerSupports> MachineConfigs { get; } =
    [
        new MachineConfigsThisCustomerSupports
        {
            Id = SolarBeeConfigId,
            DeviceId = Device1Id,
            MachineTypeId = SolarBeeTypeId,
            Name = "Ixom SolarBee",
        },
        new MachineConfigsThisCustomerSupports
        {
            Id = GsMixerConfigId,
            DeviceId = Device2Id,
            MachineTypeId = GsMixerTypeId,
            Name = "Ixom GS Mixer",
        },
    ];
}

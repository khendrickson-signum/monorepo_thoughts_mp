namespace EdisonProjectDatabase;

public static class EdisonProjectSeed
{
    public static readonly Guid Device1Id = new Guid("e936aa54-4fee-431c-9d1d-c4ee9cc5d751");

    public static readonly Guid Device2Id = new Guid("df5d2ef5-8d86-4944-badb-95e55e8ac32a");

    public static readonly Guid GrainBinReaderTypeId = new Guid("f847285a-da02-4cd2-a80c-ccadc930789b");

    public static readonly Guid CowMilkerTypeId = new Guid("246cc725-457f-4627-aab2-9bffb91ead71");

    public static readonly Guid GrainBinReaderConfigId = new Guid("efb30937-550b-44c7-baea-30cfe42640fe");

    public static readonly Guid CowMilkerConfigId = new Guid("41233157-3161-457e-85a9-ee7fd85d01c6");

    public static IReadOnlyList<DevicesForThisCustomer> Devices { get; } =
    [
        new DevicesForThisCustomer
        {
            Id = Device1Id,
            SignumGuid = new Guid("2e8f0a99-e7c0-4c30-bea2-6b242ea28084"),
            IsActive = true,
            Name = "Edison #1",
        },
        new DevicesForThisCustomer
        {
            Id = Device2Id,
            SignumGuid = new Guid("f7b0b4b3-9efb-4508-9b6b-fcfebd121ee5"),
            IsActive = true,
            Name = "Edison #2",
        },
    ];

    public static IReadOnlyList<MachineTypesForThisCustomer> MachineTypes { get; } =
    [
        new MachineTypesForThisCustomer
        {
            Id = GrainBinReaderTypeId,
            Name = "Grain Bin Reader",
        },
        new MachineTypesForThisCustomer
        {
            Id = CowMilkerTypeId,
            Name = "Cow Milker",
        },
    ];

    public static IReadOnlyList<MachineConfigsThisCustomerSupports> MachineConfigs { get; } =
    [
        new MachineConfigsThisCustomerSupports
        {
            Id = GrainBinReaderConfigId,
            DeviceId = Device1Id,
            MachineTypeId = GrainBinReaderTypeId,
            Name = "Edison Grain Bin Reader",
        },
        new MachineConfigsThisCustomerSupports
        {
            Id = CowMilkerConfigId,
            DeviceId = Device2Id,
            MachineTypeId = CowMilkerTypeId,
            Name = "Edison Cow Milker",
        },
    ];
}

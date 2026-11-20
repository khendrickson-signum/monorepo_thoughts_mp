namespace BossMasterDatabase;

public static class BossMasterSeed
{
    public static readonly Guid BobcatProjectId = new Guid("19c08948-2dcc-4076-a27a-24535a092b86");

    public static readonly Guid IxomProjectId = new Guid("390a9eeb-470b-4dec-a0a0-ac3f4e9e9d2b");

    public static readonly Guid EdisonProjectId = new Guid("b6719597-addc-4a69-882c-fc2ed1b486e0");

    public static readonly Guid BobcatSignumDevice1Id = new Guid("5962a011-138c-4e50-9d22-7dd0bb6501be");

    public static readonly Guid BobcatSignumDevice2Id = new Guid("c2165796-8976-4e4b-9719-958127a17518");

    public static readonly Guid IxomSignumDevice1Id = new Guid("1e5fc019-2561-471d-8622-357bd8675d9c");

    public static readonly Guid IxomSignumDevice2Id = new Guid("d1e12a24-a49e-41de-b4b6-0c87b67521ca");

    public static readonly Guid EdisonSignumDevice1Id = new Guid("2e8f0a99-e7c0-4c30-bea2-6b242ea28084");

    public static readonly Guid EdisonSignumDevice2Id = new Guid("f7b0b4b3-9efb-4508-9b6b-fcfebd121ee5");

    public static IReadOnlyList<AllSignumDevicesAcrossAllCustomers> Devices { get; } =
    [
        Device(BobcatSignumDevice1Id, BobcatProjectId, "Signum Bobcat IOT Device 1", "BOBCAT-IOT-1"),
        Device(BobcatSignumDevice2Id, BobcatProjectId, "Signum Bobcat IOT Device 2", "BOBCAT-IOT-2"),
        Device(IxomSignumDevice1Id, IxomProjectId, "Signum Ixom IOT Device 1", "IXOM-IOT-1"),
        Device(IxomSignumDevice2Id, IxomProjectId, "Signum Ixom IOT Device 2", "IXOM-IOT-2"),
        Device(EdisonSignumDevice1Id, EdisonProjectId, "Signum Edison IOT Device 1", "EDISON-IOT-1"),
        Device(EdisonSignumDevice2Id, EdisonProjectId, "Signum Edison IOT Device 2", "EDISON-IOT-2"),
    ];

    private static AllSignumDevicesAcrossAllCustomers Device(
        Guid id,
        Guid projectId,
        string name,
        string serialNumber)
    {
        return new AllSignumDevicesAcrossAllCustomers
        {
            Id = id,
            ProjectId = projectId,
            Name = name,
            IsActive = true,
            SerialNumber = serialNumber,
        };
    }
}

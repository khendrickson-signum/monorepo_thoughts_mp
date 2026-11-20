namespace SampleMonoRepo.DotNetApi.ClientConfigurations;

public sealed class IxomClientConfiguration : ClientConfiguration
{
    public IxomClientConfiguration()
        : base(
            ClientCode.Ixom,
            new Guid("390a9eeb-470b-4dec-a0a0-ac3f4e9e9d2b"),
            "Ixom",
            "#007A8A",
            "Server=placeholder;Database=IxomProject;User Id=placeholder;Password=placeholder;",
            "Server=placeholder;Database=IxomMaster;User Id=placeholder;Password=placeholder;")
    {
    }
}

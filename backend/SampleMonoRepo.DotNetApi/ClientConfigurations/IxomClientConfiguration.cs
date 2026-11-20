namespace SampleMonoRepo.DotNetApi.ClientConfigurations;

public sealed class IxomClientConfiguration : ClientConfiguration
{
    public IxomClientConfiguration()
        : base(
            ClientCode.Ixom,
            "Ixom",
            "#007A8A",
            "Server=placeholder;Database=IxomProject;User Id=placeholder;Password=placeholder;",
            "Server=placeholder;Database=IxomMaster;User Id=placeholder;Password=placeholder;")
    {
    }
}

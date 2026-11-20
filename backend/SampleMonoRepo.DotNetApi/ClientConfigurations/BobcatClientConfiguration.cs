namespace SampleMonoRepo.DotNetApi.ClientConfigurations;

public sealed class BobcatClientConfiguration : ClientConfiguration
{
    public BobcatClientConfiguration()
        : base(
            ClientCode.Bobcat,
            "Bobcat",
            "#E87722",
            "Server=placeholder;Database=BobcatProject;User Id=placeholder;Password=placeholder;",
            "Server=placeholder;Database=BobcatMaster;User Id=placeholder;Password=placeholder;")
    {
    }
}

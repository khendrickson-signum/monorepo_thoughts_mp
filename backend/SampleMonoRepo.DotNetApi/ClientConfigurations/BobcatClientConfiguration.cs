namespace SampleMonoRepo.DotNetApi.ClientConfigurations;

public sealed class BobcatClientConfiguration : ClientConfiguration
{
    public BobcatClientConfiguration()
        : base(
            ClientCode.Bobcat,
            new Guid("19c08948-2dcc-4076-a27a-24535a092b86"),
            "Bobcat",
            "#E87722",
            "Server=placeholder;Database=BobcatProject;User Id=placeholder;Password=placeholder;",
            "Server=placeholder;Database=BobcatMaster;User Id=placeholder;Password=placeholder;")
    {
    }
}

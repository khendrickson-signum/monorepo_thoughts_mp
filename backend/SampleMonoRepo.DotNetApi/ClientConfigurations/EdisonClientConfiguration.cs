namespace SampleMonoRepo.DotNetApi.ClientConfigurations;

public sealed class EdisonClientConfiguration : ClientConfiguration
{
    public EdisonClientConfiguration()
        : base(
            ClientCode.Edison,
            "Edison",
            "#C9A227",
            "Server=placeholder;Database=EdisonProject;User Id=placeholder;Password=placeholder;",
            "Server=placeholder;Database=EdisonMaster;User Id=placeholder;Password=placeholder;")
    {
    }
}

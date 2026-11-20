namespace SampleMonoRepo.DotNetApi.ClientConfigurations;

public sealed class EdisonClientConfiguration : ClientConfiguration
{
    public EdisonClientConfiguration()
        : base(
            ClientCode.Edison,
            new Guid("b6719597-addc-4a69-882c-fc2ed1b486e0"),
            "Edison",
            "#C9A227",
            "Server=placeholder;Database=EdisonProject;User Id=placeholder;Password=placeholder;",
            "Server=placeholder;Database=EdisonMaster;User Id=placeholder;Password=placeholder;")
    {
    }
}

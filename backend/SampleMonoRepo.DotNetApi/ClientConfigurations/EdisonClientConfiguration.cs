namespace SampleMonoRepo.DotNetApi.ClientConfigurations;

public sealed class EdisonClientConfiguration : ClientConfiguration
{
    public EdisonClientConfiguration()
        : base(
            ClientCode.Edison,
            new Guid("b6719597-addc-4a69-882c-fc2ed1b486e0"),
            "Edison",
            "#C9A227",
            "Server=localhost,30013;Database=EdisonProject;User Id=edison-project-database-user;Password=Edison-Project-Rw-1!;TrustServerCertificate=True;",
            "Server=localhost,30003;Database=BossMaster;User Id=boss-master-database-edison-user;Password=Edison-Boss-Ro-1!;TrustServerCertificate=True;")
    {
    }
}

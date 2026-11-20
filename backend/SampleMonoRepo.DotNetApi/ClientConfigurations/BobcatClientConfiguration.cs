namespace SampleMonoRepo.DotNetApi.ClientConfigurations;

// REF: c7e1e6ea-8da0-4dc8-932b-c1b8c0b460b6
public sealed class BobcatClientConfiguration : ClientConfiguration
{
    public BobcatClientConfiguration()
        : base(
            ClientCode.Bobcat,
            new Guid("19c08948-2dcc-4076-a27a-24535a092b86"),
            "Bobcat",
            "#E87722",
            "Server=localhost,30023;Database=BobcatProject;User Id=bobcat-project-database-user;Password=Bobcat-Project-Rw-1!;TrustServerCertificate=True;",
            "Server=localhost,30003;Database=BossMaster;User Id=boss-master-database-bobcat-user;Password=Bobcat-Boss-Ro-1!;TrustServerCertificate=True;")
    {
    }
}

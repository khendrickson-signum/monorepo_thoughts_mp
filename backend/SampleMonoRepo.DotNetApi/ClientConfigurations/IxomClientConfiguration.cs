namespace SampleMonoRepo.DotNetApi.ClientConfigurations;

public sealed class IxomClientConfiguration : ClientConfiguration
{
    public IxomClientConfiguration()
        : base(
            ClientCode.Ixom,
            new Guid("390a9eeb-470b-4dec-a0a0-ac3f4e9e9d2b"),
            "Ixom",
            "#007A8A",
            "Server=localhost,30033;Database=IxomProject;User Id=ixom-project-database-user;Password=Ixom-Project-Rw-1!;TrustServerCertificate=True;",
            "Server=localhost,30003;Database=BossMaster;User Id=boss-master-database-ixom-user;Password=Ixom-Boss-Ro-1!;TrustServerCertificate=True;")
    {
    }
}

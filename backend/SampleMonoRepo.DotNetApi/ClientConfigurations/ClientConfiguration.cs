namespace SampleMonoRepo.DotNetApi.ClientConfigurations;

public abstract class ClientConfiguration
{
    protected ClientConfiguration(
        ClientCode clientCode,
        Guid projectId,
        string siteTitle,
        string menuBackground,
        string projectDbConnectionString,
        string masterDbConnectionString)
    {
        ClientCode = clientCode;
        ProjectId = projectId;
        SiteTitle = siteTitle;
        MenuBackground = menuBackground;
        ProjectDbConnectionString = projectDbConnectionString;
        MasterDbConnectionString = masterDbConnectionString;
    }

    public ClientCode ClientCode { get; }

    public Guid ProjectId { get; }

    public string SiteTitle { get; }

    public string MenuBackground { get; }

    public string ProjectDbConnectionString { get; }

    public string MasterDbConnectionString { get; }
}

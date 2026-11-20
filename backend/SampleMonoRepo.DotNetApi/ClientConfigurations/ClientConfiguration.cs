namespace SampleMonoRepo.DotNetApi.ClientConfigurations;

public abstract class ClientConfiguration
{
    protected ClientConfiguration(
        ClientCode clientCode,
        string siteTitle,
        string menuBackground,
        string projectDbConnectionString,
        string masterDbConnectionString)
    {
        ClientCode = clientCode;
        SiteTitle = siteTitle;
        MenuBackground = menuBackground;
        ProjectDbConnectionString = projectDbConnectionString;
        MasterDbConnectionString = masterDbConnectionString;
    }

    public ClientCode ClientCode { get; }

    public string SiteTitle { get; }

    public string MenuBackground { get; }

    public string ProjectDbConnectionString { get; }

    public string MasterDbConnectionString { get; }
}

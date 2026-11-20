using Microsoft.Extensions.Configuration;

namespace SampleMonoRepo.DotNetApi.ClientConfigurations;

public sealed class ClientConfigurationResolver
{
    public const string SettingName = "ClientConfig";

    private readonly Lazy<ClientConfiguration> _current;

    public ClientConfigurationResolver(IConfiguration configuration)
    {
        _current = new Lazy<ClientConfiguration>(() => Resolve(configuration));
    }

    public ClientConfiguration Current => _current.Value;

    private static ClientConfiguration Resolve(IConfiguration configuration)
    {
        string? configuredClient = configuration[SettingName]?.Trim();
        if (string.IsNullOrEmpty(configuredClient))
        {
            throw new InvalidOperationException(
                $"The '{SettingName}' setting is missing. Set it to one of: {KnownClients()}.");
        }

        if (!Enum.TryParse(configuredClient, ignoreCase: true, out ClientCode clientCode)
            || !Enum.IsDefined(clientCode))
        {
            throw new InvalidOperationException(
                $"The '{SettingName}' setting '{configuredClient}' is not a known client. Expected one of: {KnownClients()}.");
        }

        return clientCode switch
        {
            ClientCode.Edison => new EdisonClientConfiguration(),
            ClientCode.Bobcat => new BobcatClientConfiguration(),
            ClientCode.Ixom => new IxomClientConfiguration(),
            _ => throw new InvalidOperationException(
                $"No client configuration is registered for '{clientCode}'.")
        };
    }

    private static string KnownClients()
    {
        return string.Join(", ", Enum.GetNames<ClientCode>());
    }
}

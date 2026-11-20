using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleMonoRepo.DotNetApi.ClientConfigurations;

namespace SampleMonoRepo.DotNetApi.Tests;

public class ClientConfigurationHostTests
{
    [Fact]
    public void Host_ResolvesClientConfigurationFromAppSettings()
    {
        using WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>();

        ClientConfiguration configuration = factory.Services.GetRequiredService<ClientConfiguration>();

        Assert.IsType<EdisonClientConfiguration>(configuration);
        Assert.Equal(ClientCode.Edison, configuration.ClientCode);
    }

    [Fact]
    public void Host_UsesSpoofedClientConfigSetting()
    {
        Dictionary<string, string?> spoofedSetting = new Dictionary<string, string?>
        {
            [ClientConfigurationResolver.SettingName] = "Ixom"
        };

        using WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, config) =>
                {
                    config.AddInMemoryCollection(spoofedSetting);
                });
            });

        ClientConfiguration configuration = factory.Services.GetRequiredService<ClientConfiguration>();

        Assert.IsType<IxomClientConfiguration>(configuration);
        Assert.Equal(ClientCode.Ixom, configuration.ClientCode);
    }
}

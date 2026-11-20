using Microsoft.Extensions.Configuration;
using SampleMonoRepo.DotNetApi.ClientConfigurations;

namespace SampleMonoRepo.DotNetApi.Tests;

public class ClientConfigurationResolverTests
{
    [Theory]
    [InlineData("Edison", typeof(EdisonClientConfiguration), ClientCode.Edison)]
    [InlineData("bobcat", typeof(BobcatClientConfiguration), ClientCode.Bobcat)]
    [InlineData("IXOM", typeof(IxomClientConfiguration), ClientCode.Ixom)]
    public void Current_DerivesConfigurationFromSetting(string setting, Type expectedType, ClientCode expectedCode)
    {
        ClientConfigurationResolver resolver = CreateResolver(setting);

        ClientConfiguration first = resolver.Current;
        ClientConfiguration second = resolver.Current;

        Assert.IsType(expectedType, first);
        Assert.Equal(expectedCode, first.ClientCode);
        Assert.Same(first, second);
    }

    [Fact]
    public void Current_ThrowsWhenSettingIsMissing()
    {
        IConfiguration configuration = new ConfigurationBuilder().Build();
        ClientConfigurationResolver resolver = new ClientConfigurationResolver(configuration);

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => resolver.Current);

        Assert.Contains(ClientConfigurationResolver.SettingName, exception.Message);
    }

    [Fact]
    public void Current_ThrowsWhenSettingIsUnknown()
    {
        ClientConfigurationResolver resolver = CreateResolver("NotAClient");

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => resolver.Current);

        Assert.Contains("NotAClient", exception.Message);
    }

    private static ClientConfigurationResolver CreateResolver(string? clientConfig)
    {
        Dictionary<string, string?> settings = new Dictionary<string, string?>
        {
            [ClientConfigurationResolver.SettingName] = clientConfig
        };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        return new ClientConfigurationResolver(configuration);
    }
}

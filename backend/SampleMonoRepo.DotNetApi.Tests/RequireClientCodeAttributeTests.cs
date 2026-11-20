using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using SampleMonoRepo.DotNetApi.ClientConfigurations;
using SampleMonoRepo.DotNetApi.Controllers;
using SampleMonoRepo.DotNetApi.Filters;

namespace SampleMonoRepo.DotNetApi.Tests;

public class RequireClientCodeAttributeTests
{
    [Theory]
    [InlineData(typeof(EdisonController), ClientCode.Edison)]
    [InlineData(typeof(BobcatController), ClientCode.Bobcat)]
    [InlineData(typeof(IxomController), ClientCode.Ixom)]
    public void Controller_DeclaresItsOwnClientCode(Type controllerType, ClientCode expectedClientCode)
    {
        RequireClientCodeAttribute? attribute = controllerType.GetCustomAttribute<RequireClientCodeAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal(expectedClientCode, attribute.ClientCode);
    }

    [Fact]
    public void GenericCustomerController_IsNotGated()
    {
        RequireClientCodeAttribute? attribute = typeof(GenericCustomerController)
            .GetCustomAttribute<RequireClientCodeAttribute>();

        Assert.Null(attribute);
    }

    [Theory]
    [InlineData(ClientCode.Edison)]
    [InlineData(ClientCode.Bobcat)]
    [InlineData(ClientCode.Ixom)]
    public void OnAuthorization_AllowsTheConfiguredClient(ClientCode clientCode)
    {
        AuthorizationFilterContext context = CreateContext(clientCode);
        RequireClientCodeAttribute attribute = new RequireClientCodeAttribute(clientCode);

        attribute.OnAuthorization(context);

        Assert.Null(context.Result);
    }

    [Theory]
    [InlineData(ClientCode.Ixom, ClientCode.Bobcat)]
    [InlineData(ClientCode.Edison, ClientCode.Ixom)]
    [InlineData(ClientCode.Bobcat, ClientCode.Edison)]
    public void OnAuthorization_RejectsADifferentClient(ClientCode runningClientCode, ClientCode requiredClientCode)
    {
        AuthorizationFilterContext context = CreateContext(runningClientCode);
        RequireClientCodeAttribute attribute = new RequireClientCodeAttribute(requiredClientCode);

        attribute.OnAuthorization(context);

        ObjectResult result = Assert.IsType<ObjectResult>(context.Result);
        ProblemDetails problem = Assert.IsType<ProblemDetails>(result.Value);
        Assert.Equal(StatusCodes.Status403Forbidden, result.StatusCode);
        Assert.Equal(StatusCodes.Status403Forbidden, problem.Status);
        Assert.Contains(requiredClientCode.ToString(), problem.Detail);
        Assert.Contains(runningClientCode.ToString(), problem.Detail);
    }

    private static AuthorizationFilterContext CreateContext(ClientCode clientCode)
    {
        ServiceCollection services = new ServiceCollection();
        services.AddSingleton<ClientConfiguration>(CreateConfiguration(clientCode));
        ServiceProvider provider = services.BuildServiceProvider();

        DefaultHttpContext httpContext = new DefaultHttpContext
        {
            RequestServices = provider
        };
        ActionContext actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

        return new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
    }

    private static ClientConfiguration CreateConfiguration(ClientCode clientCode)
    {
        return clientCode switch
        {
            ClientCode.Edison => new EdisonClientConfiguration(),
            ClientCode.Bobcat => new BobcatClientConfiguration(),
            ClientCode.Ixom => new IxomClientConfiguration(),
            _ => throw new ArgumentOutOfRangeException(nameof(clientCode), clientCode, null)
        };
    }
}

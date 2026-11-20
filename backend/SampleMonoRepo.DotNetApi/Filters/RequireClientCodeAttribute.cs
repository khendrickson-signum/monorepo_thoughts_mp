using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SampleMonoRepo.DotNetApi.ClientConfigurations;

namespace SampleMonoRepo.DotNetApi.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public sealed class RequireClientCodeAttribute : Attribute, IAuthorizationFilter
{
    public RequireClientCodeAttribute(ClientCode clientCode)
    {
        ClientCode = clientCode;
    }

    public ClientCode ClientCode { get; }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        ClientConfiguration clientConfiguration = context.HttpContext.RequestServices
            .GetRequiredService<ClientConfiguration>();

        if (clientConfiguration.ClientCode == ClientCode)
        {
            return;
        }

        string detail =
            $"This endpoint only accepts traffic when the application is configured for {ClientCode}. " +
            $"The running configuration is {clientConfiguration.ClientCode}.";

        ObjectResult result = new ObjectResult(new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Client configuration mismatch.",
            Detail = detail
        })
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
        result.ContentTypes.Add("application/problem+json");
        context.Result = result;
    }
}

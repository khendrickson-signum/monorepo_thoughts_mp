using BossMasterDatabase;
using IxomProjectDatabase;
using Microsoft.AspNetCore.Mvc;
using SampleMonoRepo.DotNetApi.ClientConfigurations;
using SampleMonoRepo.DotNetApi.Filters;

namespace SampleMonoRepo.DotNetApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[RequireClientCode(ClientCode.Ixom)]
public class IxomController : ControllerBase
{
    private readonly BossMasterDbContext _masterDbContext;
    private readonly IxomProjectDbContext _ixomProjectDbContext;

    public IxomController(BossMasterDbContext masterDbContext, IxomProjectDbContext ixomProjectDbContext)
    {
        _masterDbContext = masterDbContext;
        _ixomProjectDbContext = ixomProjectDbContext;
    }
}

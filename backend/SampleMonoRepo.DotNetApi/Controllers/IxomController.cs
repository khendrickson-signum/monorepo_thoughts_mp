using BossMasterDatabase;
using IxomProjectDatabase;
using Microsoft.AspNetCore.Mvc;

namespace SampleMonoRepo.DotNetApi.Controllers;

[ApiController]
[Route("api/[controller]")]
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

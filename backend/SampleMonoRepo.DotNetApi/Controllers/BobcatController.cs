using BobcatProjectDatabase;
using BossMasterDatabase;
using Microsoft.AspNetCore.Mvc;

namespace SampleMonoRepo.DotNetApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BobcatController : ControllerBase
{
    private readonly BossMasterDbContext _masterDbContext;
    private readonly BobcatProjectDbContext _bobcatProjectDbContext;

    public BobcatController(BossMasterDbContext masterDbContext, BobcatProjectDbContext bobcatProjectDbContext)
    {
        _masterDbContext = masterDbContext;
        _bobcatProjectDbContext = bobcatProjectDbContext;
    }
}

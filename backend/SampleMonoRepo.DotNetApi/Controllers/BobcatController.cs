using BobcatProjectDatabase;
using BossMasterDatabase;
using Microsoft.AspNetCore.Mvc;
using SampleMonoRepo.DotNetApi.ClientConfigurations;
using SampleMonoRepo.DotNetApi.Filters;

namespace SampleMonoRepo.DotNetApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[RequireClientCode(ClientCode.Bobcat)]  // fce5710e-1606-4ccb-9e5c-406cde46d156
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

using BossMasterDatabase;
using EdisonProjectDatabase;
using Microsoft.AspNetCore.Mvc;
using SampleMonoRepo.DotNetApi.ClientConfigurations;
using SampleMonoRepo.DotNetApi.Filters;

namespace SampleMonoRepo.DotNetApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[RequireClientCode(ClientCode.Edison)]
public class EdisonController : ControllerBase
{
    private readonly BossMasterDbContext _masterDbContext;
    private readonly EdisonProjectDbContext _edisonProjectDbContext;

    public EdisonController(BossMasterDbContext masterDbContext, EdisonProjectDbContext edisonProjectDbContext)
    {
        _masterDbContext = masterDbContext;
        _edisonProjectDbContext = edisonProjectDbContext;
    }
}

using BossMasterDatabase;
using EdisonProjectDatabase;
using Microsoft.AspNetCore.Mvc;

namespace SampleMonoRepo.DotNetApi.Controllers;

[ApiController]
[Route("api/[controller]")]
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

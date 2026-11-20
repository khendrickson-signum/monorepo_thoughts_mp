using BossMasterDatabase;
using Microsoft.AspNetCore.Mvc;

namespace SampleMonoRepo.DotNetApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenericCustomerController : ControllerBase
{
    private readonly BossMasterDbContext _masterDbContext;

    public GenericCustomerController(BossMasterDbContext masterDbContext)
    {
        _masterDbContext = masterDbContext;
    }
}

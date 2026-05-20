using DevArena.Entities;
using DevArena.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevArenaMostofa.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContestsController(ContestsRepo repo) : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var result = repo.GetAll();
            return Ok(result.Data);
        }
    }
}

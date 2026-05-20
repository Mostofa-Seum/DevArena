using DevArena.Repos;
using Microsoft.AspNetCore.Http;
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

        [HttpGet("GetById/{id}")]
        public IActionResult Get(int id)
        {
            var result = repo.GetById(id);
            return Ok(result.Data);
        }

        [HttpPost]
        public IActionResult Create(Contests contest)
        {
            var result = repo.Create(contest);
            if (result.HasError)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Data);

        }
    }
}

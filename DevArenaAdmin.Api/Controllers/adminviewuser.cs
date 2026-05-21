using System.Threading.Tasks; // 1. Make sure this namespace is here
using DevArena.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DevArenaAdmin.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class adminviewuser(AdminRepo repo) : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var result = repo.GetAllParticipantsAsync().GetAwaiter().GetResult();
            if (result.HasError)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Data);
        }
    }
}
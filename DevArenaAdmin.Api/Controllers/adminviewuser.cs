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
        public async Task<IActionResult> Get() // 2. Added 'async Task<'
        {
            // 3. Added 'await' here to cleanly unpack the data
            var result = await repo.GetAllParticipantsAsync();

            return Ok(result);
        }
    }
}
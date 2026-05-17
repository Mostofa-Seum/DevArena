using DevArena.Data;
using DevArena.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevArena.Repos
{
    public class ProblemsRepo(DevArenaDbContext context)
    {

        public async Task<List<Problems>> GetProblemsByContestIdAsync(int contestId)
        {
            return await context.Problems.Where(p => p.contest_id == contestId) .ToListAsync();
        }
    }
}
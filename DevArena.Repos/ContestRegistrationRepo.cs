using DevArena.Data;
using DevArena.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevArena.Repos
{
    public class ContestRegistrationRepo(DevArenaDbContext context, CurrentUserHelper currentUserHelper)
    {
        public Result<List<int>> GetRegisteredContestIds()
        {
            var result = new Result<List<int>>();
            try
            {
                int participantId = currentUserHelper.UserId;

                if (participantId <= 0)
                {
                    result.HasError = true;
                    result.Message = "Authentication error: Could not verify Participant identity.";
                    return result;
                }
                result.Data = context.ContestRegistrations
                    .Where(e => e.participant_id == participantId && e.is_active == true).Select(e => e.contest_id)
                    .ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }
    }
}
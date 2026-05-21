using DevArena.Data;
using DevArena.Shared;
using DevArena.Entities; 
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
                    .Where(e => e.participant_id == participantId && e.is_active == true)
                    .Select(e => e.contest_id)
                    .ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<bool> Join(int contestId)
        {
            var result = new Result<bool>();
            try
            {
                int participantId = currentUserHelper.UserId;

                if (participantId <= 0)
                {
                    result.HasError = true;
                    result.Message = "Authentication error: Could not verify Participant identity.";
                    return result;
                }

                // Check if a registration record already exists for this user and contest
                var existingRegistration = context.ContestRegistrations
                    .FirstOrDefault(e => e.participant_id == participantId && e.contest_id == contestId);

                if (existingRegistration != null)
                {
                    if (existingRegistration.is_active)
                    {
                        result.HasError = true;
                        result.Message = "You are already registered for this contest.";
                        return result;
                    }
                    else
                    {
                        existingRegistration.is_active = true;
                    }
                }
                else
                {
                    var newRegistration = new ContestRegistration
                    {
                        participant_id = participantId,
                        contest_id = contestId,
                        is_active = true
                    };
                    context.ContestRegistrations.Add(newRegistration);
                }

                context.SaveChanges();
                result.Data = true;
            }
            catch (Exception e)
            {
                result.HasError = true;
                // Using InnerException helps catch exact database Foreign Key errors
                result.Message = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }

        public Result<bool> Leave(int contestId)
        {
            var result = new Result<bool>();
            try
            {
                int participantId = currentUserHelper.UserId;

                if (participantId <= 0)
                {
                    result.HasError = true;
                    result.Message = "Authentication error: Could not verify Participant identity.";
                    return result;
                }

                // Find the specific active registration
                var existingRegistration = context.ContestRegistrations
                    .FirstOrDefault(e => e.participant_id == participantId && e.contest_id == contestId);

                if (existingRegistration == null)
                {
                    result.HasError = true;
                    result.Message = "Registration not found.";
                    return result;
                }

                existingRegistration.is_active = false;

                context.SaveChanges();
                result.Data = true;
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }
    }
}
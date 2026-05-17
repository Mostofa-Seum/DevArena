using DevArena.Data;
using DevArena.Entities;
using DevArena.Models;
using DevArena.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevArena.Repos
{
    public class HostRepo(DevArenaDbContext context)
    {
        public Result<List<Host>> GetAll()
        {
            var result = new Result<List<Host>>();
            try
            {
                result.Data = context.Hosts.ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<Host?> GetById(int id)
        {
            var result = new Result<Host?>();
            try
            {
                result.Data = context.Hosts.FirstOrDefault(c => c.id == id);
                {
                    result.HasError = true;
                    result.Message = "Host not found.";
                }
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<Host?> Authenticate(string email, string password)
        {
            var result = new Result<Host?>();
            try
            {
                result.Data = context.Hosts.FirstOrDefault(e => e.email == email && e.password == password);
                if (result.Data == null)
                {
                    result.HasError = true;
                    result.Message = "Invalid email or password.";
                }
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<Host> Save(Host model)
        {
            var result = new Result<Host>();
            try
            {
                // Adjust 'title' if your Host entity uses a different property for validation
                if (context.Hosts.Any(c => c.name == model.name && c.id != model.id))
                {
                    result.HasError = true;
                    result.Message = "Host with the same name already exists.";
                    return result;
                }

                var objToSave = context.Hosts.Find(model.id);

                if (objToSave == null)
                {
                    objToSave = new Host();
                    context.Hosts.Add(objToSave);
                }

                // Update these mapping fields to match your actual Host entity properties
                objToSave.name = model.name;
                objToSave.email = model.email;
                objToSave.password = model.password;
                objToSave.Is_active = model.Is_active;
                objToSave.created_at = model.created_at;


                context.SaveChanges();
                result.Data = objToSave;
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }
            return result;
        }

        public Result<bool> Delete(int id)
        {
            var result = new Result<bool>();
            try
            {
                var objToDelete = context.Hosts.Find(id);
                if (objToDelete == null)
                {
                    result.HasError = true;
                    result.Message = "Host not found.";
                    return result;
                }
                context.Hosts.Remove(objToDelete);
                context.SaveChanges();
                result.Data = true;
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }
            return result;
        }
        public Result<List<RegistrationModel>> GetParticipantRegistrations(int hostId)
        {
            var result = new Result<List<RegistrationModel>>();
            try
            {
                result.Data = (from e in context.ContestRegistrations
                               join c in context.Contests on e.contest_id equals c.id
                               join p in context.Participants on e.participant_id equals p.id
                               where c.host_id == hostId && e.is_active == true

                               // Check if the participant exists in the Judges table using their email
                               let isJudge = context.Judges.Any(j => j.participant_id == p.id && j.contest_id == c.id && j.Is_active == true)

                               select new RegistrationModel
                               {
                                   ContestId = c.id,
                                   ParticipantId = p.id,
                                   ContestName = c.title,
                                   ParticipantName = p.name,
                                   ParticipantEmail = p.email,
                                   // Set the Role based on the check above
                                   Role = isJudge ? "Judge" : "Participant"
                               }).ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<bool> Remove(int contestId, int participantId)
        {
            var result = new Result<bool>();
            try
            {
                // 1. Find the specific registration row
                var registration = context.ContestRegistrations
                    .FirstOrDefault(cr => cr.contest_id == contestId && cr.participant_id == participantId);

                // 2. If it exists, remove it from the database completely
                if (registration != null)
                {
                    context.ContestRegistrations.Remove(registration);
                    context.SaveChanges();
                }

                result.Data = true;
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }
            return result;
        }

        public Result<bool> PromoteToJudge(int participantId, int contestId, int hostId)
        {
            try
            {
                // Check if a judge record already exists for this participant & contest
                var existingJudge = context.Judges
                    .FirstOrDefault(j => j.participant_id == participantId && j.contest_id == contestId);

                if (existingJudge != null)
                {
                    if (existingJudge.Is_active)
                    {
                        // They are already an active judge, no action needed
                        return Result<bool>.Failure("This participant is already an active judge for this contest.");
                    }
                    else
                    {
                        // They exist but are inactive. Reactivate them!
                        existingJudge.Is_active = true;
                        existingJudge.promoted_by_host_id = hostId; // Update who promoted them

                        context.Judges.Update(existingJudge);
                        context.SaveChanges();

                        return Result<bool>.Success(true);
                    }
                }

                // If they don't exist at all, create a brand new judge record
                var newJudge = new Judge
                {
                    participant_id = participantId,
                    contest_id = contestId,
                    promoted_by_host_id = hostId,
                    Is_active = true,
                    created_at = DateTime.UtcNow
                };

                context.Judges.Add(newJudge);
                context.SaveChanges();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                // If the database throws an error (like a foreign key issue), we catch it here
                return Result<bool>.Failure($"Failed to promote to judge: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        public Result<bool> DeactivateParticipant(int participantId, int contestId)
        {
            try
            {
                var registration = context.ContestRegistrations
                    .FirstOrDefault(cr => cr.participant_id == participantId && cr.contest_id == contestId);

                if (registration == null)
                {
                    return Result<bool>.Failure("Registration not found.");
                }
                registration.is_active = false;

                context.ContestRegistrations.Update(registration);
                context.SaveChanges();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Failed to remove participant: {ex.Message}");
            }
        }
    }
}
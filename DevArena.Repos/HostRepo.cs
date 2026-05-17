using System;
using System.Collections.Generic;
using System.Linq;
using DevArena.Data;
using DevArena.Entities;
using DevArena.Shared;
using DevArena.Models;

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
                // We use LINQ to JOIN the three tables together based on their IDs
                result.Data = (from e in context.ContestRegistrations
                               join c in context.Contests on e.contest_id equals c.id
                               join p in context.Participants on e.participant_id equals p.id
                               // Only get active registrations for contests owned by this specific host
                               where c.host_id == hostId && e.is_active == true
                               select new RegistrationModel
                               {
                                   ContestId = c.id,          
                                   ParticipantId = p.id,
                                   ContestName = c.title,
                                   ParticipantName = p.name,
                                   ParticipantEmail = p.email

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

        public Result<bool> Promote(int contestId, int participantId)
        {
            var result = new Result<bool>();
            try
            {
                var participant = context.Participants.Find(participantId);

                if (participant == null)
                {
                    result.HasError = true;
                    result.Message = "Participant not found.";
                    return result;
                }

                bool alreadyJudge = context.Judges.Any(j => j.email == participant.email);

                if (!alreadyJudge)
                {
                    // 3. Create the new Judge using the Participant's exact details
                    var newJudge = new Judge
                    {
                        name = participant.name,
                        email = participant.email,
                        password = participant.password, // Keep the same login password
                        is_active = true,
                        created_at = DateTime.UtcNow
                    };
                    context.Judges.Add(newJudge);
                }

                // 4. (Optional but recommended) Remove them from this contest's participants list 
                // since they are now a Judge and shouldn't be competing!
                var registration = context.ContestRegistrations
                    .FirstOrDefault(cr => cr.contest_id == contestId && cr.participant_id == participantId);

                if (registration != null)
                {
                    context.ContestRegistrations.Remove(registration);
                }

                // 5. Save all changes to the database at once
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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using DevArena.Data;
using DevArena.Entities;
using DevArena.Shared;
//need to work here to make sure that the repo is working with the correct entity and that the properties
//are mapped correctly in the Save method. Also, ensure that the validation logic in the Save method is
//appropriate for the Contests entity, such as checking for unique titles or other relevant fields.
namespace DevArena.Repos
{
    public class ParticipantsRepo(DevArenaDbContext context)
    {
        public Result<List<Contests>> GetAll()
        {
            var result = new Result<List<Contests>>();
            try
            {
                result.Data = context.Contests.ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<Contests?> GetById(int id)
        {
            var result = new Result<Contests?>();
            try
            {
                result.Data = context.Contests.FirstOrDefault(c => c.id == id);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<Contests> Save(Contests model)
        {
            var result = new Result<Contests>();
            try
            {
                // Adjust 'title' if your Contests entity uses a different property for validation
                if (context.Contests.Any(c => c.title == model.title && c.id != model.id))
                {
                    result.HasError = true;
                    result.Message = "Contest with the same title already exists.";
                    return result;
                }

                var objToSave = context.Contests.Find(model.id);

                if (objToSave == null)
                {
                    objToSave = new Contests();
                    context.Contests.Add(objToSave);
                }

                // Update these mapping fields to match your actual Contests entity properties
                objToSave.title = model.title;
                objToSave.start_time = model.start_time;
                objToSave.end_time = model.end_time;
                objToSave.is_active = model.is_active;
                objToSave.host_id = 1; // Set this to the appropriate host_id based on your application logic


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
                var objToDelete = context.Contests.Find(id);
                if (objToDelete == null)
                {
                    result.HasError = true;
                    result.Message = "Contest not found.";
                    return result;
                }
                context.Contests.Remove(objToDelete);
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
using System;
using System.Collections.Generic;
using System.Linq;
using DevArena.Data;
using DevArena.Entities;
using DevArena.Shared;

namespace DevArena.Repos
{
    public class ContestsRepo(DevArenaDbContext context, CurrentUserHelper currentUserHelper)
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


        public Result<List<Contests>> GetActive()
        {
            var result = new Result<List<Contests>>();
            try
            {
                result.Data = context.Contests.Where(c => c.is_active==true).ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<List<Contests>> GetInActive()
        {
            var result = new Result<List<Contests>>();
            try
            {
                result.Data = context.Contests.Where(c => c.is_active == false).ToList();
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
                result.Data = context.Contests.FirstOrDefault(c => c.id == id && c.host_id == currentUserHelper.UserId);
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
                objToSave.host_id = currentUserHelper.UserId;


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

        public Result<List<Contests>> GetAllByCurrentHost()
        {
            var result = new Result<List<Contests>>();
            try
            {
                
                result.Data = context.Contests
                    .Where(c => c.host_id == currentUserHelper.UserId)
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
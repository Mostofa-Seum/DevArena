using System;
using System.Collections.Generic;
using System.Linq;
using DevArena.Data;
using DevArena.Entities;
using DevArena.Shared;

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
                if(result.Data == null)
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
    }
}
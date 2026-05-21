using System;
using System.Collections.Generic;
using System.Linq;
using DevArena.Data;
using DevArena.Entities;
using DevArena.Shared;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DevArena.Repos
{
    public class AdminRepo(DevArenaDbContext context)
    {
        public async Task<Result<Admin>> GetAdminByEmailAsync(string email)
        {
            var result = new Result<Admin>();
            try
            {
                result.Data = await context.Admins.FirstOrDefaultAsync(a => a.email == email);
                if (result.Data == null)
                {
                    result.HasError = true;
                    result.Message = "Admin not found.";
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<Result<int>> GetCountAsync<T>() where T : class
        {
            var result = new Result<int>();
            try
            {
                result.Data = await context.Set<T>().CountAsync();
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<Result<List<Contests>>> GetRecentContestsAsync(int count)
        {
            var result = new Result<List<Contests>>();
            try
            {
                result.Data = await context.Contests
                    .OrderByDescending(c => c.id)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<Result<Admin>> UpdateAdminAsync(Admin model)
        {
            var result = new Result<Admin>();
            try
            {
                var admin = await context.Admins.FirstOrDefaultAsync(a => a.id == model.id);
                if (admin == null)
                {
                    result.HasError = true;
                    result.Message = "Admin not found.";
                    return result;
                }

                admin.name = model.name;
                admin.email = model.email;
                if (!string.IsNullOrEmpty(model.password))
                {
                    admin.password = model.password;
                }

                context.Update(admin);
                await context.SaveChangesAsync();
                result.Data = admin;
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = ex.Message;
            }
            return result;
        }

        // Participants Management
        public async Task<Result<List<Participants>>> GetAllParticipantsAsync()
        {
            var result = new Result<List<Participants>>();
            try
            {
                result.Data = await context.Participants.ToListAsync();
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<Result<bool>> ToggleParticipantStatusAsync(int id)
        {
            var result = new Result<bool>();
            try
            {
                var p = await context.Participants.FindAsync(id);
                if (p != null)
                {
                    p.Is_active = !p.Is_active;
                    await context.SaveChangesAsync();
                    result.Data = p.Is_active;
                }
                else
                {
                    result.HasError = true;
                    result.Message = "Participant not found.";
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<Result<bool>> DeleteParticipantAsync(int id)
        {
            var result = new Result<bool>();
            try
            {
                var p = await context.Participants.FindAsync(id);
                if (p != null)
                {
                    context.Participants.Remove(p);
                    await context.SaveChangesAsync();
                    result.Data = true;
                }
                else
                {
                    result.HasError = true;
                    result.Message = "Participant not found.";
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = ex.Message;
            }
            return result;
        }

        // Host Management
        public async Task<Result<List<Host>>> GetAllHostsAsync()
        {
            var result = new Result<List<Host>>();
            try
            {
                result.Data = await context.Hosts.ToListAsync();
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<Result<bool>> ToggleHostStatusAsync(int id)
        {
            var result = new Result<bool>();
            try
            {
                var h = await context.Hosts.FindAsync(id);
                if (h != null)
                {
                    h.Is_active = !h.Is_active;
                    await context.SaveChangesAsync();
                    result.Data = h.Is_active;
                }
                else
                {
                    result.HasError = true;
                    result.Message = "Host not found.";
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<Result<bool>> DeleteHostAsync(int id)
        {
            var result = new Result<bool>();
            try
            {
                var h = await context.Hosts.FindAsync(id);
                if (h != null)
                {
                    context.Hosts.Remove(h);
                    await context.SaveChangesAsync();
                    result.Data = true;
                }
                else
                {
                    result.HasError = true;
                    result.Message = "Host not found.";
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = ex.Message;
            }
            return result;
        }

        // Contest Management
        public async Task<Result<List<Contests>>> GetAllContestsAsync()
        {
            var result = new Result<List<Contests>>();
            try
            {
                result.Data = await context.Contests.Include(c => c.Hosts).ToListAsync();
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<Result<bool>> CancelContestAsync(int id)
        {
            var result = new Result<bool>();
            try
            {
                var c = await context.Contests.FindAsync(id);
                if (c != null)
                {
                    c.is_active = false;
                    await context.SaveChangesAsync();
                    result.Data = true;
                }
                else
                {
                    result.HasError = true;
                    result.Message = "Contest not found.";
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<Result<bool>> DeleteContestAsync(int id)
        {
            var result = new Result<bool>();
            try
            {
                var c = await context.Contests.FindAsync(id);
                if (c != null)
                {
                    context.Contests.Remove(c);
                    await context.SaveChangesAsync();
                    result.Data = true;
                }
                else
                {
                    result.HasError = true;
                    result.Message = "Contest not found.";
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = ex.Message;
            }
            return result;
        }
    }
}
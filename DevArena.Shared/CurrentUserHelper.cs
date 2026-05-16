using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DevArena.Shared
{
    public class CurrentUserHelper(IHttpContextAccessor accessor)
    {
        public bool IsAuthenticated
        {
            get
            {
                try
                {
                    return (bool)accessor.HttpContext?.User?.Identity?.IsAuthenticated;
                }
                catch(Exception)
                {
                    return false;
                }
            }
        }
        public int UserId
        {
            get
            {
                try
                {
                    var claim = accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
                    return claim != null ? int.Parse(claim.Value) : -1;
                }
                catch (Exception)
                {
                    return -1;
                }
            }
        }

        public String Email
        {
            get
            {
                try
                {
                    var email = accessor.HttpContext?.User?.FindFirst("Email").Value;
                    return email != null ? email : string.Empty;
                }
                catch (Exception e)
                {
                    return "-";
                }
            }
        }
    }
}

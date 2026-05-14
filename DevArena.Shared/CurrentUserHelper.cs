using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
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
                catch(Exception e)
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
                    var id = accessor.HttpContext?.User?.FindFirst("UserID").Value;
                    return id!=null ? int.Parse(id) : -1;
                }
                catch (Exception e)
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

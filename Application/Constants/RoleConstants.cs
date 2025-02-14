using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Constants
{
    public static class RoleConstants
    {
        public static readonly HashSet<string> AvailableRoles = new HashSet<string>
        {
            "Admin",
            "User",
        };
    }
}
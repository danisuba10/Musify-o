using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string UserName { get; set; }
        public string? PasswordHash { get; set; }
        public string Role { get; set; } = "User";
        public string DisplayName { get; set; } = String.Empty;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Exceptions.User
{
    public class UserDoesNotExistException : Exception
    {
        public UserDoesNotExistException() : base("User does not exist with this email!") { }
        public UserDoesNotExistException(string message) : base(message) { }
        public UserDoesNotExistException(string message, Exception innerException) : base(message, innerException) { }
    }
}
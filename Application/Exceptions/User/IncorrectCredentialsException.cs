using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Exceptions.User
{
    public class IncorrectCredentialsException : Exception
    {
        public IncorrectCredentialsException() : base("Credentials does not match for user!") { }
        public IncorrectCredentialsException(string message) : base(message) { }
        public IncorrectCredentialsException(string message, Exception innerException) : base(message, innerException) { }
    }
}
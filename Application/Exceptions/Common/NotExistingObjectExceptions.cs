using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Exceptions.Common
{
    public class NotExistingObjectExceptions : Exception
    {
        public NotExistingObjectExceptions() : base("Object does not exist!") { }
        public NotExistingObjectExceptions(string ObjectType) : base($"{ObjectType} does not exist!") { }
        public NotExistingObjectExceptions(string ObjectType, Exception innerException) : base($"{ObjectType} does not exist!", innerException) { }
    }
}
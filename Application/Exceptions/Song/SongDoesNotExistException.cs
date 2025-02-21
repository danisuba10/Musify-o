using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Exceptions.Song
{
    public class SongDoesNotExistException : Exception
    {
        public SongDoesNotExistException() : base("Song does not exist with this id!") { }
        public SongDoesNotExistException(string message) : base(message) { }
        public SongDoesNotExistException(string message, Exception innerException) : base(message, innerException) { }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DataTransferObjects.Responses
{
    public class ArtistResponse
    {
        public Guid? Id { get; set; } = null;
        public String Name { get; set; } = "";
    }
}
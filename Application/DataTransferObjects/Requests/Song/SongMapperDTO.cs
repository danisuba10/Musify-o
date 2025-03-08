using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;

namespace Application.DataTransferObjects.Requests
{
    public class SongMapperDTO
    {
        public required Song Song { get; set; }
        public int? PositionInPlaylist { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Application.DataTransferObjects.Responses
{
    public class AlbumResponse
    {
        public Guid? Id { get; set; } = null;
        public String Name { get; set; } = "";
        public List<Song>? Songs { get; set; } = null;
        public List<Artist>? Artists { get; set; } = null;
        public List<Guid> ArtistIds { get; set; } = new List<Guid>();
    }
}
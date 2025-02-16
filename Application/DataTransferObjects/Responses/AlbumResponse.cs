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
        public int Year { get; set; } = 0;
        public string ImageLocation { get; set; } = "";
        public List<Guid> SongIds { get; set; } = new List<Guid>();
        public List<SongResponse>? Songs { get; set; } = null;
        public List<Guid> ArtistIds { get; set; } = new List<Guid>();
        public List<ArtistResponse>? Artists { get; set; } = null;
    }
}
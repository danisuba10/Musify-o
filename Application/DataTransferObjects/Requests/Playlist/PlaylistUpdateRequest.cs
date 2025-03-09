using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Application.Services.Converters;
using Domain;
using Microsoft.AspNetCore.Http;

namespace Application.DataTransferObjects.Requests
{
    public class PlaylistUpdateRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
        [JsonConverter(typeof(VisibilityConverter))]
        public Visibility? Visibility { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;

namespace Application.DataTransferObjects.Requests
{
    public class PlaylistWithImageAccent
    {
        public required Playlist Playlist { get; set; }
        public ImageAccent? ImageAccent { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;

namespace Application.DataTransferObjects.Requests
{
    public class UserWithImageAccent
    {
        public required Domain.User Playlist { get; set; }
        public ImageAccent? ImageAccent { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DataTransferObjects.Responses;
using Domain;

namespace Application.Mappers
{
    public class UserMapper
    {
        public static UserResponseCompact mapToResponseCompact(User user)
        {
            return new UserResponseCompact
            {
                Id = user.Id,
                DisplayName = user.DisplayName
            };
        }
    }
}
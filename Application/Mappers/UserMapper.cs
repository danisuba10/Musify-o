using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DataTransferObjects.Responses;
using AutoMapper;
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
                DisplayName = String.IsNullOrWhiteSpace(user.DisplayName) ? user.Email : user.DisplayName
            };
        }

        public static UserResponseCompactWImage mapToResponseCompactWithImage(User user, ImageAccent? imageAccent)
        {
            UserResponseCompactWImage response = new UserResponseCompactWImage
            {
                Id = user.Id,
                Image = new ImageResponse { ImageLocation = user.ImageLocation },
                Name = String.IsNullOrWhiteSpace(user.DisplayName) ? user.Email : user.DisplayName,
            };

            if (imageAccent != null)
            {
                response.Image.LowColor = imageAccent.LowAccent;
                response.Image.MiddleColor = imageAccent.MiddleAccent;
                response.Image.HighColor = imageAccent.HighAccent;
            }

            return response;
        }

        public static ProfileResponse mapToProfileRespose(User user, ImageAccent? imageAccent)
        {
            ProfileResponse response = new ProfileResponse
            {
                Id = user.Id,
                Image = new ImageResponse { ImageLocation = user.ImageLocation },
                Name = String.IsNullOrWhiteSpace(user.DisplayName) ? user.Email : user.DisplayName,
                Followers = 0
            };

            if (imageAccent != null)
            {
                response.Image.LowColor = imageAccent.LowAccent;
                response.Image.MiddleColor = imageAccent.MiddleAccent;
                response.Image.HighColor = imageAccent.HighAccent;
            }

            return response;
        }
    }
}
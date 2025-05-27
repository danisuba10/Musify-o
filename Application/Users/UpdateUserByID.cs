using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Constants;
using Application.DataTransferObjects.Requests;
using Application.Images;
using MediatR;
using Persistence;

namespace Application.Users
{
    public class UpdateUserByID
    {
        public class Query : IRequest
        {
            public required UpdateUserRequest Request { get; set; }
            public string? ImageFolderPath { get; set; }
        }
        public class Handler : IRequestHandler<Query>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMediator _mediator;
            public Handler(ApplicationDbContext context, IMediator mediator)
            {
                _context = context;
                _mediator = mediator;
            }
            public async Task<Unit> Handle(Query query, CancellationToken cancellationToken)
            {
                var user = _context.Users
                    .FirstOrDefault(u => u.Id == query.Request.Id);

                if (user == null)
                {
                    throw new Exception("User with this ID does not exist!");
                }

                if (query.Request.IsTwoFactorEnabled.HasValue)
                {
                    user.IsTwoFactorEnabled = query.Request.IsTwoFactorEnabled.Value;
                }

                if (!String.IsNullOrWhiteSpace(query.Request.DisplayName))
                {
                    user.DisplayName = query.Request.DisplayName;
                }

                if (!String.IsNullOrWhiteSpace(query.Request.Role))
                {
                    if (!RoleConstants.AvailableRoles.Contains(query.Request.Role))
                    {
                        throw new Exception("Update user exception: Invalid role!");
                    }
                    user.Role = query.Request.Role;
                }

                if (!String.IsNullOrWhiteSpace(query.Request.Email))
                {
                    user.Email = query.Request.Email;
                }

                if (query.Request.File != null)
                {
                    try
                    {
                        if (String.IsNullOrWhiteSpace(query.ImageFolderPath))
                        {
                            throw new ArgumentNullException("Image folder path not set, even though we are trying to upload image.");
                        }
                        await _mediator.Send(new UploadImage.Command { Name = query.Request.Id.ToString(), formFile = query.Request.File, Path = Path.Combine(query.ImageFolderPath, "user"), RootImagePath = query.ImageFolderPath });
                        user.ImageLocation = Path.Combine("user", user.Id + ".jpg");
                    }
                    catch (ArgumentNullException an)
                    {
                        throw new Exception("Update user error: " + an.Message);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Update user error: Image upload failed:\n", ex);
                    }
                }

                _context.Users.Update(user);
                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}
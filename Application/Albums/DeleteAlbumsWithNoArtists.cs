using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Persistence;

namespace Application.Albums
{
    public class DeleteAlbumsWithNoArtists
    {
        public class Command : IRequest<Unit>
        {
            public required string ImageFolderPath { get; set; }
            public required string SoundFolderPath { get; set; }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMediator _mediator;
            public Handler(ApplicationDbContext context, IMediator mediator)
            {
                _mediator = mediator;
            }
            public async Task<Unit> Handle(Command cmd, CancellationToken cancellationToken)
            {
                var albums = _context.Albums
                    .Where(a => !_context.AlbumArtistRelations.Any(ar => ar.AlbumId == a.Id))
                    .ToList();

                if (albums != null && albums.Count > 0)
                {
                    foreach (Album album in albums)
                    {
                        await _mediator.Send(new RemoveAlbumByID.Command { Id = album.Id, ImageFolderPath = cmd.ImageFolderPath, SoundFolderPath = cmd.SoundFolderPath });
                    }
                }

                return Unit.Value;
            }
        }
    }
}
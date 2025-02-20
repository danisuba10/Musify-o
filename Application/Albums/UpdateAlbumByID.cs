using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Images;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Albums
{
    public class UpdateAlbumByID
    {
        public class Query : IRequest
        {
            public Guid Id { get; set; }
            public string? Name { get; set; }
            public int? Year { get; set; }
            public IFormFile? File { get; set; }
            public List<Guid>? ArtistIds { get; set; }
            public string ImageFolderPath { get; set; }
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
                int artistsNotAdded = 0;

                if (query.Name == null && query.File == null && query.Year == null && (query.ArtistIds == null || query.ArtistIds.Count == 0))
                {
                    throw new Exception("No new data supplied! Change could not be made!");
                }

                var existingAlbum = await _context.Albums
                    .FirstOrDefaultAsync(a => a.Id == query.Id, cancellationToken);

                if (existingAlbum == null)
                {
                    throw new Exception("Album does not exist!");
                }

                if (!String.IsNullOrWhiteSpace(query.Name))
                {
                    Console.WriteLine(query.Name);
                    existingAlbum.Name = query.Name;
                }

                if (query.File != null)
                {
                    try
                    {
                        await _mediator.Send(new UploadImage.Command { Name = query.Id.ToString(), formFile = query.File, Path = Path.Combine(query.ImageFolderPath, "album") });
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Update album error: Image upload failed:\n", ex);
                    }
                }

                if (query.Year != null)
                {
                    existingAlbum.ReleaseYear = (int)query.Year;
                }

                if (!(query.ArtistIds == null || query.ArtistIds.Count == 0))
                {
                    try
                    {
                        artistsNotAdded = await _mediator.Send(new AddArtistsToAlbum.Query
                        { AlbumId = query.Id, ArtistIds = query.ArtistIds, Replace = true }, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("UpdateAlbum exception for adding artists: " + ex.Message);
                    }
                }

                _context.Albums.Update(existingAlbum);
                await _context.SaveChangesAsync();

                if (artistsNotAdded > 0)
                {
                    throw new Exception("Update Album: Album modified, however some artists could not be added.");
                }

                return Unit.Value;
            }
        }
    }
}
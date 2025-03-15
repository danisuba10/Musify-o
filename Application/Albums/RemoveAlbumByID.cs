using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Images;
using Application.Songs;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application
{
    public class RemoveAlbumByID
    {
        public class Command : IRequest<string>
        {
            public Guid Id { get; set; }
            public required string ImageFolderPath { get; set; }
            public required string SoundFolderPath { get; set; }
        }
        public class Handler : IRequestHandler<Command, string>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMediator _mediator;
            public Handler(ApplicationDbContext context, IMediator mediator)
            {
                _context = context;
                _mediator = mediator;
            }
            public async Task<string> Handle(Command request, CancellationToken cancellationToken)
            {
                var album = await _context.Albums
                    .Include(a => a.Songs)
                    .Include(a => a.AlbumArtistRelations)
                    .FirstOrDefaultAsync(album => album.Id == request.Id, cancellationToken);

                if (album == null)
                {
                    throw new Exception("Album does not exist!");
                }

                string imageLocation = album.ImageLocation;

                var songs = album.Songs;

                foreach (Song song in songs)
                {
                    try
                    {
                        await _mediator.Send(new RemoveSongByID.Command { Id = song.Id, SoundFolderPath = request.SoundFolderPath });
                    }
                    catch (FileNotFoundException e) { Console.WriteLine(e); }
                }

                var albumArtistRelations = album.AlbumArtistRelations.ToList();
                if (albumArtistRelations != null && albumArtistRelations.Count > 0)
                {
                    _context.AlbumArtistRelations.RemoveRange(albumArtistRelations);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                try
                {
                    await _mediator.Send(new DeleteImage.Command { Path = Path.Combine(request.ImageFolderPath, imageLocation) });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                _context.Albums.Remove(album);
                await _context.SaveChangesAsync(cancellationToken);

                return imageLocation;
            }
        }
    }
}
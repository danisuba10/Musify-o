using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Playlists
{
    public class GetTopPlaylistsOfUser
    {
        public class Query : IRequest<List<Playlist>>
        {
            public required Guid UserId { get; set; }
            public Guid? RequesterUserId { get; set; }
            public string? RequesterRole { get; set; }
        }
        public class Handler : IRequestHandler<Query, List<Playlist>>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<List<Playlist>> Handle(Query query, CancellationToken cancellationToken)
            {
                var playlistsQuery = _context.Playlists.AsQueryable();

                playlistsQuery = playlistsQuery.Where(pl => pl.UserId == query.UserId);

                if (query.RequesterUserId == null)
                {
                    playlistsQuery = playlistsQuery.Where(p => p.Visibility == Visibility.Public);
                }
                else if (!(query.RequesterRole?.Equals("Admin") ?? false))
                {
                    playlistsQuery = playlistsQuery.Where(p => p.Visibility == Visibility.Public || p.UserId == query.UserId);
                }

                playlistsQuery = playlistsQuery
                    .OrderBy(pl => pl.Name)
                    .Take(25);

                return await playlistsQuery.ToListAsync(cancellationToken);
            }
        }
    }
}
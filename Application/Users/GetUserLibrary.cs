using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.DataTransferObjects.Responses;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Users
{
    public class GetUserLibrary
    {
        public class Query : IRequest<LibraryPageResponse>
        {
            public required Guid UserId { get; set; }
            public int PageSize { get; set; } = 25;
            public string? LastSavedAt { get; set; }
            public string? LastItemId { get; set; }
        }

        public class Handler : IRequestHandler<Query, LibraryPageResponse>
        {
            private readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<LibraryPageResponse> Handle(Query q, CancellationToken ct)
            {
                IQueryable<UserLibraryItem> query = _context.UserLibraryItems
                    .Where(uli => uli.UserId == q.UserId);

                if (!string.IsNullOrEmpty(q.LastSavedAt) && !string.IsNullOrEmpty(q.LastItemId))
                {
                    var lastSavedAtParsed = DateTime.Parse(q.LastSavedAt);
                    var lastItemIdParsed = Guid.Parse(q.LastItemId);

                    query = query.Where(uli =>
                        uli.SavedAt < lastSavedAtParsed ||
                        (uli.SavedAt == lastSavedAtParsed && string.Compare(uli.Id.ToString(), lastItemIdParsed.ToString()) > 0)
                    );
                }

                query = query
                    .OrderByDescending(uli => uli.SavedAt)
                    .ThenBy(uli => uli.Id)
                    .Take(q.PageSize);

                var items = await query.ToListAsync(ct);

                // Batched lookups for entity details
                var albumItems = items.Where(i => i.ItemType == "Album").ToList();
                var artistItems = items.Where(i => i.ItemType == "Artist").ToList();
                var playlistItems = items.Where(i => i.ItemType == "Playlist").ToList();

                var albumIds = albumItems.Select(a => a.ItemId).ToList();
                var artistIds = artistItems.Select(a => a.ItemId).ToList();
                var playlistIds = playlistItems.Select(a => a.ItemId).ToList();

                var albumsDict = albumIds.Any()
                    ? await _context.Albums
                        .Where(a => albumIds.Contains(a.Id))
                        .ToDictionaryAsync(a => a.Id, a => a, ct)
                    : new Dictionary<Guid, Album>();

                var artistsDict = artistIds.Any()
                    ? await _context.Artists
                        .Where(a => artistIds.Contains(a.Id))
                        .ToDictionaryAsync(a => a.Id, a => a, ct)
                    : new Dictionary<Guid, Artist>();

                var playlistsDict = playlistIds.Any()
                    ? await _context.Playlists
                        .Include(p => p.User)
                        .Where(p => playlistIds.Contains(p.Id))
                        .ToDictionaryAsync(p => p.Id, p => p, ct)
                    : new Dictionary<Guid, Playlist>();

                var result = new List<LibraryItemResponse>();

                foreach (var item in items)
                {
                    string name = "";
                    string? imageLocation = null;
                    string? subtitle = null;

                    if (item.ItemType == "Album" && albumsDict.TryGetValue(item.ItemId, out var album))
                    {
                        name = album.Name;
                        imageLocation = album.ImageLocation;
                        subtitle = "Album";
                    }
                    else if (item.ItemType == "Artist" && artistsDict.TryGetValue(item.ItemId, out var artist))
                    {
                        name = artist.Name;
                        imageLocation = artist.ImageLocation;
                        subtitle = "Artist";
                    }
                    else if (item.ItemType == "Playlist" && playlistsDict.TryGetValue(item.ItemId, out var playlist))
                    {
                        name = playlist.Name;
                        imageLocation = playlist.ImageLocation;
                        subtitle = $"Playlist • {playlist.User?.DisplayName ?? "Unknown"}";
                    }

                    result.Add(new LibraryItemResponse
                    {
                        Id = item.Id,
                        ItemId = item.ItemId,
                        ItemType = item.ItemType,
                        Name = name,
                        ImageLocation = imageLocation,
                        Subtitle = subtitle,
                        SavedAt = item.SavedAt
                    });
                }

                string? lastSavedAt = null;
                string? lastItemId = null;

                if (result.Count >= q.PageSize)
                {
                    var lastItem = items.Last();
                    lastSavedAt = lastItem.SavedAt.ToString("o");
                    lastItemId = lastItem.Id.ToString();
                }

                return new LibraryPageResponse
                {
                    Items = result,
                    LastSavedAt = lastSavedAt,
                    LastItemId = lastItemId
                };
            }
        }
    }
}

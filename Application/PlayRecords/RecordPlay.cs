using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Persistence;

namespace Application.PlayRecords
{
    public class RecordPlay
    {
        public class Query : IRequest<Unit>
        {
            public required Guid PlayedItemId { get; set; }
            public required PlayedItemType PlayedItemType { get; set; }
            public required Guid UserId { get; set; }
            public DateTime? TimeStamp { get; set; }
        }

        public class Handler : IRequestHandler<Query, Unit>
        {
            ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Query request, CancellationToken cancellationToken)
            {
                Console.WriteLine(request.PlayedItemType.ToString());
                PlayRecord playRecord = request.PlayedItemType switch
                {
                    PlayedItemType.Song => new SongPlayRecord
                    {
                        PlayedItemId = request.PlayedItemId,
                        UserId = request.UserId,
                        Timestamp = request.TimeStamp ?? DateTime.UtcNow,
                    },
                    PlayedItemType.Album => new AlbumPlayRecord
                    {
                        PlayedItemId = request.PlayedItemId,
                        UserId = request.UserId,
                        Timestamp = request.TimeStamp ?? DateTime.UtcNow,
                    },
                    PlayedItemType.Playlist => new PlaylistPlayRecord
                    {
                        PlayedItemId = request.PlayedItemId,
                        UserId = request.UserId,
                        Timestamp = request.TimeStamp ?? DateTime.UtcNow,
                    },
                    _ => throw new ArgumentException("Record play: Invalid PlayedItemType!")
                };

                _context.PlayRecords.Add(playRecord);
                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }

    }
}
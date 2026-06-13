using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Users
{
    public class ToggleLibraryItem
    {
        public class Command : IRequest<bool>
        {
            public required Guid UserId { get; set; }
            public required Guid ItemId { get; set; }
            public required string ItemType { get; set; }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<bool> Handle(Command cmd, CancellationToken cancellationToken)
            {
                var existing = await _context.UserLibraryItems
                    .FirstOrDefaultAsync(uli =>
                        uli.UserId == cmd.UserId &&
                        uli.ItemId == cmd.ItemId &&
                        uli.ItemType == cmd.ItemType,
                        cancellationToken);

                if (existing != null)
                {
                    _context.UserLibraryItems.Remove(existing);
                    await _context.SaveChangesAsync(cancellationToken);
                    return false;
                }
                else
                {
                    try
                    {
                        var item = new UserLibraryItem
                        {
                            UserId = cmd.UserId,
                            ItemId = cmd.ItemId,
                            ItemType = cmd.ItemType,
                            SavedAt = DateTime.UtcNow
                        };

                        _context.UserLibraryItems.Add(item);
                        await _context.SaveChangesAsync(cancellationToken);
                        return true;
                    }
                    catch (DbUpdateException ex) when (ex.InnerException != null && ex.InnerException.Message.Contains("duplicate"))
                    {
                        return true;
                    }
                }
            }
        }
    }
}

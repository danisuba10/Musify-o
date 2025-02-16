using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.ImageAccents
{
    public class GetImageAccentByPath
    {
        public class Query : IRequest<ImageAccent?>
        {
            public string Path { get; set; }
        }

        public class Handler : IRequestHandler<Query, ImageAccent?>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<ImageAccent?> Handle(Query query, CancellationToken cancellationToken)
            {
                return await _context.ImageAccents
                    .FirstOrDefaultAsync(ia => ia.ImagePath == query.Path, cancellationToken);
            }
        }
    }
}
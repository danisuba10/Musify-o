using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Exceptions.Common;
using MediatR;

namespace Application.Sounds
{
    public class DeleteSound
    {
        public class Command : IRequest<Unit>
        {
            public required string Path { get; set; }
        }
        public class Handler : IRequestHandler<Command, Unit>
        {
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var filePath = request.Path;

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Console.WriteLine("Sound deleted!!");
                }
                else
                {
                    Console.WriteLine("Sound does not exist!!");
                    throw new FileNotFoundException("File does not exist at path!");
                }

                return Unit.Value;
            }
        }
    }

}
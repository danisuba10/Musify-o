using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Search
{
    public class SearchResultResponse
    {
        public required string Type { get; set; }
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public string? ImageLocation { get; set; }
    }
}
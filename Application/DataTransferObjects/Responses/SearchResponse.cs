using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DataTransferObjects.Responses
{
    public class SearchResponse
    {
        public required List<SearchResult> SearchResults { get; set; }
        public string? LastName { get; set; }
        public DateTime? LastCreatedAt { get; set; }
    }
}
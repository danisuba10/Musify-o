using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DataTransferObjects.Requests
{
    public class SearchRequest
    {
        public required string SearchTerm { get; set; }
        public string? LastName { get; set; }
        public DateTime? LastCreatedAt { get; set; }
        public int PageSize { get; set; }
    }
}
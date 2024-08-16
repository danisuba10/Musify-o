using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Search
{
    public class SearchResult
    {
        public required string Type { get; set; }
        public required int Id { get; set; }
        public required string Name { get; set; }
        public string? ImgPath { get; set; }
    }
}
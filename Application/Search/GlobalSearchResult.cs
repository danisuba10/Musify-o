using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DataTransferObjects;

namespace Application.Search
{
    public class GlobalSearchResult
    {
        public List<SearchResult> Artists { get; set; } = new List<SearchResult>();
        public List<SearchResult> Albums { get; set; } = new List<SearchResult>();
        public List<SearchResult> Songs { get; set; } = new List<SearchResult>();
        public List<SearchResult> Playlists { get; set; } = new List<SearchResult>();
    }
}
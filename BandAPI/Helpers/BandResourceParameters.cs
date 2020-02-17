using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BandAPI.Helpers
{
    public class BandResourceParameters
    {
        const int maxPageSize = 20;

        public string MainGenre { get; set; }
        public string SearchQuery { get; set; }
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 2;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value; // Setting Page Size here
        }


        public string OrderBy { get; set; } = "Name"; // Defalt orderBy
        public string Fields { get; set; }
    }
}

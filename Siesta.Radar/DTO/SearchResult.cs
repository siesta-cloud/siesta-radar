using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siesta.Radar.DTO
{
    public class SearchResult
    {
        public List<Flight> flights = new List<Flight>();

        public class Flight
        {
            public string flyFrom { get; set; }
            public string flyTo { get; set; }
            public int price { get; set; }
            public string cityFrom { get; set; }
            public string cityTo { get; set; }
            public double distance { get; set; }
            public string deep_link { get; set; }
            public DateTime local_arrival { get; set; }
            public DateTime local_departure { get; set; }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Siesta.Kiwi.Model
{
   public class SearchResultResponse
    {

        public class SearchParams
        {
            public string to_type { get; set; }
            public string flyFrom_type { get; set; }
        }

        public class Baglimit
        {
            public int hand_height { get; set; }
            public int hand_length { get; set; }
            public int hand_weight { get; set; }
            public int hand_width { get; set; }
            public int hold_height { get; set; }
            public int hold_length { get; set; }
            public int hold_weight { get; set; }
            public int hold_width { get; set; }
            public int hold_dimensions_sum { get; set; }
        }

        //public class BagsPrice
        //{
        //}

        public class Conversion
        {
            public int EUR { get; set; }
        }

        public class CountryFrom
        {
            public string code { get; set; }
            public string name { get; set; }
        }

        public class CountryTo
        {
            public string code { get; set; }
            public string name { get; set; }
        }

        public class Duration
        {
            public int departure { get; set; }
            public int @return { get; set; }
            public int total { get; set; }
        }

        public class Route
        {
            public string airline { get; set; }
            public bool bags_recheck_required { get; set; }
            public string cityFrom { get; set; }
            public string cityTo { get; set; }
            public string combination_id { get; set; }
            public object equipment { get; set; }
            public string fare_basis { get; set; }
            public string fare_classes { get; set; }
            public string fare_family { get; set; }
            public int flight_no { get; set; }
            public string flyFrom { get; set; }
            public string flyTo { get; set; }
            public bool guarantee { get; set; }
            public string id { get; set; }
            public DateTime last_seen { get; set; }
            public DateTime local_arrival { get; set; }
            public DateTime local_departure { get; set; }
            public object operating_carrier { get; set; }
            public DateTime refresh_timestamp { get; set; }
            public int @return { get; set; }
            public DateTime utc_arrival { get; set; }
            public DateTime utc_departure { get; set; }
            public string vehicle_type { get; set; }
        }

        public class Data
        {
            public List<string> airlines { get; set; }
            //public Baglimit baglimit { get; set; }
            //public List<decimal> bags_price { get; set; }
            public string booking_token { get; set; }
            public string cityFrom { get; set; }
            public string cityTo { get; set; }
            //public Conversion conversion { get; set; }
            public CountryFrom countryFrom { get; set; }
            public CountryTo countryTo { get; set; }
            public string deep_link { get; set; }
            public double distance { get; set; }
            public Duration duration { get; set; }
            //public bool facilitated_booking_available { get; set; }
            public string flyFrom { get; set; }
            public string flyTo { get; set; }
            //public bool has_airport_change { get; set; }
            //public string id { get; set; }
            public DateTime local_arrival { get; set; }
            public DateTime local_departure { get; set; }
            //public int nightsInDest { get; set; }
            //public int pnr_count { get; set; }
            public int price { get; set; }
            //public double quality { get; set; }
            //public double rank { get; set; }
            //public List<Route> route { get; set; }
            //public List<List<string>> routes { get; set; }
            //public List<object> transfers { get; set; }
            //public List<string> type_flights { get; set; }
            //public DateTime utc_arrival { get; set; }
            //public DateTime utc_departure { get; set; }
        }

        //public class Value
        //{
        //    public SearchParams search_params { get; set; }
        //    public int time { get; set; }
        //    public List<object> connections { get; set; }
        //    public string currency { get; set; }
        //    public double currency_rate { get; set; }
        //    public Data data { get; set; }
        //}

        public class Response
        {
            //public Value value { get; set; }
            public string search_id { get; set; }            
            public List<Data> data { get; set; }
        }

    }
}

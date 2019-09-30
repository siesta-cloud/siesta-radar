using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Siesta.Kiwi.Model;
using Siesta.Radar.DTO;
using Siesta.SentinelHub;

namespace Siesta.Radar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        int simulationAmount = 15;

        [HttpGet("{upperRight}/{bottomLeft}/{fromLocation}/{interestType}")]
        public ActionResult<SearchResult> Search(string upperRight, string bottomLeft, string fromLocation, string interestType)
        {
            var charsToRemove = new string[] { "(", ")", " " };
            foreach (var c in charsToRemove)
            {
                upperRight = upperRight.Replace(c, string.Empty);
                bottomLeft = bottomLeft.Replace(c, string.Empty);

            }
            //string fff = upperRight.Split(",")[0];
            //decimal ddddd = Convert.ToDecimal(fff);
            decimal upperRightLon = Convert.ToDecimal(upperRight.Split(",")[1], CultureInfo.InvariantCulture);
            decimal upperRightLat = Convert.ToDecimal(upperRight.Split(",")[0], CultureInfo.InvariantCulture);
            decimal bottomLeftLon = Convert.ToDecimal(bottomLeft.Split(",")[1], CultureInfo.InvariantCulture);
            decimal bottomLeftLat = Convert.ToDecimal(bottomLeft.Split(",")[0], CultureInfo.InvariantCulture);
            List<Airport> airports = GetAirports();

            airports = FilterVisibleAirports(airports, upperRightLon, upperRightLat, bottomLeftLon, bottomLeftLat);


            // Get Snow coeficients
            switch (interestType)
            {
                case "snow":
                    GetSnowCoeficients(airports.Take(simulationAmount).ToList());
                    break;
                case "air-quality":

                    break;
                case "water-quality":
                    GetWaterCoeficients(airports.Take(simulationAmount).ToList());
                    break;
                default:
                    break;
            }

            Siesta.Kiwi.Kiwi kiwi = new Siesta.Kiwi.Kiwi();

            SearchResult result = new SearchResult();
            foreach (var item in airports.Take(simulationAmount))
            {

                if (interestType == "snow")
                {
                    if (item.SnowCoeficient < 0.05m)
                    {
                        continue;
                    }
                }
                if (interestType == "water-quality")
                {
                    if (item.WaterCoeficient < 0.05m)
                    {
                        continue;
                    }
                }
                SearchResultResponse.Response kiwiResult = kiwi.Search(DateTime.Now, DateTime.Now.AddDays(3), fromLocation, item.code);
                if (kiwiResult != null)
                {
                    foreach (var flightData in kiwiResult.data.Take(1))
                    {
                        SearchResult.Flight flight = new SearchResult.Flight();
                        flight.cityFrom = flightData.cityFrom;
                        flight.cityTo = flightData.cityTo;
                        flight.deep_link = flightData.deep_link;
                        flight.distance = flightData.distance;
                        flight.flyFrom = flightData.flyFrom;
                        flight.flyTo = flightData.flyTo;
                        flight.local_arrival = flightData.local_arrival;
                        flight.local_departure = flightData.local_departure;
                        flight.price = flightData.price;
                        result.flights.Add(flight);
                    }
                }
            }

            return result;

        }

        // GET api/values
        [HttpGet("satellite")]
        public ActionResult<IEnumerable<decimal>> Satellite()
        {

            SentinelHubConnector sentinel = new SentinelHubConnector();
            return sentinel.GetSentinel2BPosition();

        }


        private void GetSnowCoeficients(List<Airport> airports)
        {
            GetCoeficients(airports, "SNOW");
        }


        private void GetWaterCoeficients(List<Airport> airports)
        {
            GetCoeficients(airports, "WATER");
        }
        
        private void GetCoeficients(List<Airport> airports, string type)
        {
            foreach (Airport airport in airports)
            {
                var airportLon = Double.Parse(airport.lon, CultureInfo.InvariantCulture);
                var airportLat = Double.Parse(airport.lat, CultureInfo.InvariantCulture);

                var upperRightLon = airportLon + 0.5;
                var upperRightLat = airportLat + 0.5;

                var bottomLeftLon = airportLon - 0.5;
                var bottomLeftLat = airportLat - 0.5;

                if (upperRightLon > 180) upperRightLon -= 360;
                if (upperRightLat > 90) upperRightLat -= 180;

                if (bottomLeftLon < -180) bottomLeftLon += 360;
                if (bottomLeftLat < -90) bottomLeftLat += 180;
                // TODO create radius and serach if there is snow
                SentinelHubConnector sentinel = new SentinelHubConnector();
                
                switch(type)
                {
                    case "SNOW":
                        airport.SnowCoeficient = sentinel.GetSnowData(bottomLeftLon, bottomLeftLat, upperRightLon, upperRightLat);
                        break;
                    
                    case "WATER":
                        airport.WaterCoeficient = sentinel.GetWaterData(bottomLeftLon, bottomLeftLat, upperRightLon, upperRightLat);
                        break;

                }               
            };
        }

        private List<Airport> FilterVisibleAirports(List<Airport> airports, decimal upperRightLon, decimal upperRightLat, decimal bottomLeftLon, decimal bottomLeftLat)
        {
            List<Airport> filteredAirports = new List<Airport>();
            foreach (var item in airports)
            {
                if (IsWithin(upperRightLon, upperRightLat, bottomLeftLon, bottomLeftLat, item))
                {
                    filteredAirports.Add(item);
                }
            }
            return filteredAirports;
        }

        public static Boolean IsWithin(decimal upperRightLon, decimal upperRightLat, decimal bottomLeftLon, decimal bottomLeftLat, Airport airport)
        {
            var airportLon = Decimal.Parse(airport.lon, CultureInfo.InvariantCulture);
            var airportLat = Decimal.Parse(airport.lat, CultureInfo.InvariantCulture);

            var isLongInRange = upperRightLon < bottomLeftLon ? airportLon >= bottomLeftLon || airportLon <= upperRightLon
                                                              : airportLon >= bottomLeftLon && airportLon <= upperRightLon;

            return airportLat >= bottomLeftLat && airportLat <= upperRightLat && isLongInRange;
        }

        private List<Airport> GetAirports()
        {
            using (var client = new HttpClient())
            {

                var response = client.GetAsync("https://dumysiesta.blob.core.windows.net/copernicus/airports.json").Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;

                    // by calling .Result you are synchronously reading the result
                    string responseString = responseContent.ReadAsStringAsync().Result;
                    List<Airport> result = JsonConvert.DeserializeObject<List<Airport>>(responseString);


                    return result;
                }
                else
                {
                    return null;
                }
            }
        }
    }


}
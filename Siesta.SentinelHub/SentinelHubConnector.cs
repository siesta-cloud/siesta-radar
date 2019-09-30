using System;
using System.Collections;
using System.Collections.Generic;
using RestSharp;
using Newtonsoft.Json;
using System.Linq;

namespace Siesta.SentinelHub
{
    public class SentinelHubConnector
    {
        public readonly string _secret = "xXxsecretxXx";
        public readonly string _id = "7f065198-b9ce-4f43-aabf-b6e0e6b2ab5d";
        public readonly string _snowId = "6a68bd00-219d-4a4b-a71f-86849a656cc7";
        public readonly string _airId = "dd678fc0-da10-46d5-b927-d5f069d37456";
        public readonly string _waterId = "c46728de-87fb-4b67-b632-9a202ba5ba4d";
        public readonly string _fisEndpoint = "http://services.sentinel-hub.com/ogc/fis/";

        public readonly string _newFisEndpoint = "https://creodias.sentinel-hub.com/ogc/fis";

        public string GetAuthenticationToken()
        {
            var client = new RestClient("https://services.sentinel-hub.com/oauth/token");

            var request = new RestRequest()
            {
                Method = Method.POST
            };
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", _id);
            request.AddParameter("client_secret", _secret);

            var response = client.Execute(request);
            var token = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content)["access_token"].ToString();

            return token;
        }

        public decimal GetSnowData(double bottomLeftLon, double bottomLeftLat, double upperRightLon, double upperRightLat)
        {
            return GetData(bottomLeftLon, bottomLeftLat, upperRightLon, upperRightLat, "SNOWDATA");
        }

        public decimal GetAirData(double bottomLeftLon, double bottomLeftLat, double upperRightLon, double upperRightLat)
        {
            return GetData(bottomLeftLon, bottomLeftLat, upperRightLon, upperRightLat, "CO_VISUALIZED");
        }

        public decimal GetWaterData(double bottomLeftLon, double bottomLeftLat, double upperRightLon, double upperRightLat)
        {
            return GetData(bottomLeftLon, bottomLeftLat, upperRightLon, upperRightLat, "WATERDATA");
        }

        public decimal GetData(double bottomLeftLon, double bottomLeftLat, double upperRightLon, double upperRightLat, string layer)
        {
            var viewId = "";
            var endpoint = "";

            switch (layer)
            {
                case "SNOWDATA":
                    viewId = _snowId;
                    endpoint = _fisEndpoint;
                    break;

                case "CO_VISUALIZED":
                    viewId = _airId;
                    endpoint = _newFisEndpoint;
                    break;

                case "WATERDATA":
                    viewId = _waterId;
                    endpoint = _fisEndpoint;
                    break;
            }

            var client = new RestClient(endpoint + viewId);

            var request = new RestRequest()
            {
                Method = Method.GET
            };

            var coordinates = $"{bottomLeftLon.ToString().Replace(",", ".")},{bottomLeftLat.ToString().Replace(",", ".")},{upperRightLon.ToString().Replace(",", ".")},{upperRightLat.ToString().Replace(",", ".")}";

            request.AddQueryParameter("BBOX", coordinates);
            request.AddQueryParameter("LAYER", layer);
            request.AddQueryParameter("RESOLUTION", "60");
            request.AddQueryParameter("MAXCC", "20");
            request.AddQueryParameter("CRS", "EPSG:4326");
            request.AddQueryParameter("TIME", "2019-08-30/2019-09-28");

            var response = client.Execute(request);
            if (response.Content == null) return 0;
            var responseJson = JsonConvert.DeserializeObject<Output>(response.Content.Replace("\"NaN\"", "0.0"));

            if (responseJson.C0 == null)
            {
                return 0;
            }
            return layer == "WATERDATA" ? responseJson.C0.First().BasicStats.StDev : responseJson.C0.First().BasicStats.Mean;
        }

        public List<decimal> GetSentinel2BPosition()
        {
            return GetSatellitePosition("42063");
        }

        public List<decimal> GetSentinel2APosition()
        {
            return GetSatellitePosition("40697");
        }

        public List<decimal> GetSatellitePosition(string id)
        {
            var client = new RestClient($"https://www.n2yo.com/rest/v1/satellite/positions/{id}/41.702/-76.014/0/2/&apiKey=9AAKXJ-CQRE5H-7YWW9B-47J7");

            var request = new RestRequest()
            {
                Method = Method.GET
            };
            var response = client.Execute(request);

            var responseJson = JsonConvert.DeserializeObject<SentinelLocation>(response.Content);

            var location = new List<decimal>();
            location.Add(responseJson.Positions.First().Satlatitude);
            location.Add(responseJson.Positions.First().Satlongitude);

            return location;
        }
    }
}

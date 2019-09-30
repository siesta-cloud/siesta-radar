using Newtonsoft.Json;
using Siesta.Kiwi.Model;
using System;
using System.Net;
using System.Net.Http;

namespace Siesta.Kiwi
{
    public class Kiwi
    {
        // Constants
        const string server = "https://kiwicom-prod.apigee.net/";
        const string apiKey = "apiKey";


        public SearchResultResponse.Response Search(DateTime from, DateTime to, string fromCode, string toCode)
        {

            // Handler to decompress Gzip        
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };


            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("apikey", apiKey);
                var response = client.GetAsync(server + "v2/search?fly_from=" + fromCode + "&fly_to=" + toCode + "&date_from=05%2F12%2F2019&date_to=10%2F12%2F2019&return_from=20%2F12%2F2019&return_to=25%2F12%2F2019&nights_in_dst_from=2&nights_in_dst_to=14&max_fly_duration=20&flight_type=round&adults=1&max_stopovers=2&vehicle_type=aircraft&one_for_city=1").Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;

                    // by calling .Result you are synchronously reading the result
                    string responseString = responseContent.ReadAsStringAsync().Result;
                    SearchResultResponse.Response responseData = JsonConvert.DeserializeObject<SearchResultResponse.Response>(responseString);


                    return responseData;
                }
                else
                {
                    return null;
                }
            }
        }


    }
}

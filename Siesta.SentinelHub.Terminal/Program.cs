using System;

namespace Siesta.SentinelHub.Terminal
{
    class Program
    {
        static void Main(string[] args)
        {
            var connector = new SentinelHubConnector();
            //Console.WriteLine(connector.GetWaterData(48.971527, 15.898574, 49.971527, 16.898574));
            //Console.WriteLine(connector.GetWaterData(47.697956,16.223885,48.697956,17.223885));
            //Console.WriteLine(connector.GetWaterData(40.8304, -8.7231, 40.6655, -8.72126));
            Console.WriteLine(connector.GetSentinel2BPosition()[1]);
        }
    }
}

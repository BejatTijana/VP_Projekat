using System;
using System.ServiceModel;

namespace Service
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(WeatherService));
            host.Open();
            Console.WriteLine("[Server] WCF servis pokrenut. Ceka na klijente...");
            Console.WriteLine("Pritisni Enter za gasenje.");
            Console.ReadLine();
            host.Close();
        }
    }
}
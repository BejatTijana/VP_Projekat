using System;
using System.ServiceModel;

namespace Service
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(WeatherService));

            try
            {
                host.Open();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("==============================================");
                Console.WriteLine("   Meteoroloska stanica - Servis pokrenut    ");
                Console.WriteLine("==============================================");
                Console.ResetColor();

               

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Pritisnite Enter za gasenje servisa...");
                Console.WriteLine("==============================================");
                Console.ResetColor();

                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Greska pri pokretanju servisa: {e.Message}");
                Console.ResetColor();
            }
            finally
            {
                if (host.State == CommunicationState.Opened)
                {
                    host.Close();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Servis ugasen.");
                    Console.ResetColor();
                }
            }
        }
    }
}
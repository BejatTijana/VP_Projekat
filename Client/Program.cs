using System;
using System.Collections.Generic;
using System.ServiceModel;
using Common;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<IWeatherService> factory =
                new ChannelFactory<IWeatherService>("WeatherService");
            IWeatherService proxy = factory.CreateChannel();

            Console.WriteLine("Unesite putanju do CSV fajla:");
            string csvPath = Console.ReadLine();

            List<WeatherSample> samples;
            using (CsvParser parser = new CsvParser(csvPath))
            {
                samples = parser.ParseFirst107Rows();
            }

            Console.WriteLine($"[Client] Ucitano {samples.Count} uzoraka. Saljem na server...");

            try
            {
                string ack = proxy.StartSession(new SessionMeta
                {
                    StationName = "Stanica-1",
                    Description = "Simulacija meteoroloskih merenja"
                });
                Console.WriteLine($"[Client] StartSession: {ack}");

                foreach (WeatherSample sample in samples)
                {
                    string result = proxy.PushSample(sample);
                    Console.WriteLine($"[Client] PushSample: {result}");
                }

                string endAck = proxy.EndSession();
                Console.WriteLine($"[Client] EndSession: {endAck}");
            }
            catch (FaultException<DataFormatFault> e)
            {
                Console.WriteLine($"[GRESKA] DataFormatFault: {e.Detail.Message}");
            }
            catch (FaultException<ValidationFault> e)
            {
                Console.WriteLine($"[GRESKA] ValidationFault: {e.Detail.Message}");
            }

            Console.WriteLine("Gotovo! Pritisni Enter za izlaz.");
            Console.ReadLine();
        }
    }
}
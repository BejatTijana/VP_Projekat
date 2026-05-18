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
            ChannelFactory<IWeatherService> factory = null;
            IWeatherService proxy = null;
            CsvReader csvReader = null;

            try
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("==============================================");
                Console.WriteLine("   Meteoroloska stanica - Klijent pokrenut   ");
                Console.WriteLine("==============================================");
                Console.ResetColor();

                
                csvReader = new CsvReader();
                List<WeatherSample> samples = csvReader.ReadSamples();

                if (samples.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Nema validnih uzoraka za slanje. Gasenje klijenta...");
                    Console.ResetColor();
                    return;
                }

                
                factory = new ChannelFactory<IWeatherService>("WeatherService");
                proxy = factory.CreateChannel();

                
                SessionMeta meta = new SessionMeta(
                    Guid.NewGuid().ToString(),
                    DateTime.Now,
                    samples.Count);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n==============================================");
                Console.WriteLine($"Pokretanje sesije: {meta.SessionId}");
                Console.WriteLine("==============================================");
                Console.ResetColor();

                WeatherServiceResponse startResponse = proxy.StartSession(meta);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"StartSession -> {startResponse}");
                Console.ResetColor();

                if (startResponse.ResponseStatus == ResponseStatus.NACK)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Sesija nije mogla da se pokrene. Gasenje klijenta...");
                    Console.ResetColor();
                    return;
                }

                
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n==============================================");
                Console.WriteLine("Slanje uzoraka...");
                Console.WriteLine("==============================================");
                Console.ResetColor();

                int successCount = 0;
                int failCount = 0;

                foreach (WeatherSample sample in samples)
                {
                    try
                    {
                        WeatherServiceResponse pushResponse = proxy.PushSample(sample);

                        if (pushResponse.ResponseStatus == ResponseStatus.ACK)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"PushSample -> {pushResponse}");
                            Console.ResetColor();
                            successCount++;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"PushSample -> {pushResponse}");
                            Console.ResetColor();
                            failCount++;
                        }
                    }
                    catch (FaultException<DataFormatFault> e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[DATA FORMAT FAULT] {e.Detail}");
                        Console.ResetColor();
                        failCount++;
                    }
                    catch (FaultException<ValidationFault> e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[VALIDATION FAULT] {e.Detail}");
                        Console.ResetColor();
                        failCount++;
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[ERROR] {e.Message}");
                        Console.ResetColor();
                        failCount++;
                    }
                }

                
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n==============================================");
                Console.WriteLine("Zavrsavanje sesije...");
                Console.WriteLine("==============================================");
                Console.ResetColor();

                WeatherServiceResponse endResponse = proxy.EndSession();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"EndSession -> {endResponse}");
                Console.ResetColor();

                
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n==============================================");
                Console.WriteLine("              SUMMARY                        ");
                Console.WriteLine("==============================================");
                Console.ResetColor();

                Console.WriteLine($"Ukupno uzoraka:     {samples.Count}");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Uspesno poslato:    {successCount}");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Neuspesno poslato:  {failCount}");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("==============================================");
                Console.ResetColor();
            }
            catch (FaultException<DataFormatFault> e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[DATA FORMAT FAULT] {e.Detail}");
                Console.ResetColor();
            }
            catch (FaultException<ValidationFault> e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[VALIDATION FAULT] {e.Detail}");
                Console.ResetColor();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] {e.Message}");
                Console.ResetColor();
            }
            finally
            {
                if (csvReader != null)
                {
                    csvReader.Dispose();
                }
                if (factory != null && factory.State == CommunicationState.Opened)
                {
                    factory.Close();
                }
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\nPritisnite Enter za izlaz...");
                Console.ResetColor();
                Console.ReadLine();
            }
        }
    }
}
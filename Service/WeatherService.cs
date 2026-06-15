using System;
using System.ServiceModel;
using Common;

namespace Service
{
    class WeatherService : IWeatherService
    {
        private static SessionManager sessionManager = new SessionManager();

        public WeatherServiceResponse StartSession(SessionMeta meta)
        {
            try
            {
                if (meta == null || string.IsNullOrEmpty(meta.SessionId))
                {
                    throw new FaultException<DataFormatFault>(
                        new DataFormatFault("Meta zaglavlje je null ili prazno.",
                        "SessionMeta"));
                }

                return sessionManager.OpenSession(meta);
            }
            catch (FaultException<DataFormatFault>)
            {
                throw;
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] {e.Message}");
                Console.ResetColor();
                return new WeatherServiceResponse(
                    ResponseStatus.NACK,
                    SessionStatus.IN_PROGRESS,
                    $"Greska pri pokretanju sesije: {e.Message}");
            }
        }

        public WeatherServiceResponse PushSample(WeatherSample sample)
        {
            try
            {
                if (sample == null)
                {
                    throw new FaultException<DataFormatFault>(
                        new DataFormatFault("Uzorak je null.", "WeatherSample"));
                }

                ValidateSample(sample);

                return sessionManager.WriteSample(sample);
            }
            catch (FaultException<ValidationFault> ex)
            {
                sessionManager.WriteReject(sample, ex.Detail.Message);
                throw;
            }
            catch (FaultException<DataFormatFault>)
            {
                throw;
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] {e.Message}");
                Console.ResetColor();
                return new WeatherServiceResponse(
                    ResponseStatus.NACK,
                    SessionStatus.IN_PROGRESS,
                    $"Greska pri prijemu uzorka: {e.Message}");
            }
        }

        public WeatherServiceResponse EndSession()
        {
            try
            {
                return sessionManager.CloseSession();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] {e.Message}");
                Console.ResetColor();
                return new WeatherServiceResponse(
                    ResponseStatus.NACK,
                    SessionStatus.COMPLETED,
                    $"Greska pri zatvaranju sesije: {e.Message}");
            }
        }

        private void ValidateSample(WeatherSample sample)
        {
            if (sample.Pressure <= 0)
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault(
                        "Pritisak mora biti veci od 0.",
                        "Pressure",
                        sample.Pressure.ToString()));
            }

            if (sample.Temperature < -100 || sample.Temperature > 100)
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault(
                        "Temperatura mora biti izmedju -100 i 100.",
                        "Temperature",
                        sample.Temperature.ToString()));
            }

            if (sample.VPact < 0)
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault(
                        "VPact ne moze biti negativan.",
                        "VPact",
                        sample.VPact.ToString()));
            }

            if (sample.VPdef < 0)
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault(
                        "VPdef ne moze biti negativan.",
                        "VPdef",
                        sample.VPdef.ToString()));
            }
        }
    }
}
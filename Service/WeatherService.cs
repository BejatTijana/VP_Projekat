using System;
using System.ServiceModel;
using Common;

namespace Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class WeatherService : IWeatherService
    {
        private SessionManager sessionManager = new SessionManager();
        private WeatherAnalyzer analyzer = new WeatherAnalyzer();
        private string currentStation = "";

        public WeatherService()
        {
            sessionManager.OnTransferStarted += (s, e) =>
                Console.WriteLine($"[EVENT] Prenos zapocet - Stanica: {e.SessionName}");

            sessionManager.OnTransferCompleted += (s, e) =>
                Console.WriteLine($"[EVENT] Prenos zavrsen - Stanica: {e.SessionName}");

            sessionManager.OnSampleReceived += (s, e) =>
                Console.WriteLine($"[EVENT] Primljen uzorak: T={e.Sample.T}, P={e.Sample.Pressure}");

            analyzer.OnWarningRaised += (s, e) =>
                Console.WriteLine($"[UPOZORENJE] {e.ParameterName} - {e.Direction} ocekivane vrijednosti. Vrijednost: {e.Value}");
        }

        public string StartSession(SessionMeta meta)
        {
            if (meta == null || string.IsNullOrEmpty(meta.StationName))
                throw new FaultException<DataFormatFault>(new DataFormatFault("Meta zaglavlje je nevalidno."));

            currentStation = meta.StationName;
            sessionManager.StartSession(meta);
            return "ACK - IN_PROGRESS";
        }

        public string PushSample(WeatherSample sample)
        {
            if (sample == null)
                throw new FaultException<ValidationFault>(new ValidationFault("Uzorak je null."));

            if (sample.Pressure <= 0)
            {
                sessionManager.SaveReject(sample, "Pritisak mora biti pozitivan.");
                return "NACK - Nevalidan pritisak";
            }

            sessionManager.SaveSample(sample);
            analyzer.Analyze(sample);
            return "ACK";
        }

        public string EndSession()
        {
            sessionManager.EndSession(currentStation);
            sessionManager.Dispose();
            return "ACK - COMPLETED";
        }
    }
}
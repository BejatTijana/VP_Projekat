using System;
using System.ServiceModel;
using Common;

namespace Service
{
    class SessionManager
    {
        private FileManager fileManager;
        private WeatherAnalytics analytics;
        private string currentSessionId;

        public SessionManager()
        {
            analytics = new WeatherAnalytics();
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            analytics.OnTransferStarted += OnTransferStarted;
            analytics.OnTransferCompleted += OnTransferCompleted;
            analytics.OnSampleReceived += OnSampleReceived;
            analytics.OnPressureSpike += OnWarningRaised;
            analytics.OnVPActSpike += OnWarningRaised;
            analytics.OnVPDefSpike += OnWarningRaised;
            analytics.OnOutOfBandWarning += OnWarningRaised;
        }

        private void OnTransferStarted(object sender, TransferEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[TRANSFER STARTED] {e.Message}");
            Console.ResetColor();
        }

        private void OnTransferCompleted(object sender, TransferEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[TRANSFER COMPLETED] {e.Message}");
            Console.ResetColor();
        }

        private void OnSampleReceived(object sender, SampleEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[SAMPLE RECEIVED] {e.Message}");
            Console.ResetColor();
        }

        private void OnWarningRaised(object sender, WarningEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[WARNING] {e.Message}");
            Console.ResetColor();
        }

        public WeatherServiceResponse OpenSession(SessionMeta meta)
        {
            currentSessionId = meta.SessionId;
            analytics.Reset();

            fileManager = new FileManager();
            fileManager.InitializeSession();

            analytics.RaiseTransferStarted(currentSessionId);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[SESSION STARTED] {meta}");
            Console.ResetColor();

            return new WeatherServiceResponse(
                ResponseStatus.ACK,
                SessionStatus.IN_PROGRESS,
                $"Sesija {currentSessionId} uspesno zapoceta.");
        }

        public WeatherServiceResponse WriteSample(WeatherSample sample)
        {
            fileManager.AppendSample(sample);

            analytics.RaiseSampleReceived(sample);
            analytics.AnalyzeSample(sample);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[PRENOS U TOKU...] {sample.Date}");
            Console.ResetColor();

            return new WeatherServiceResponse(
                ResponseStatus.ACK,
                SessionStatus.IN_PROGRESS,
                $"Uzorak {sample.Date} uspesno primljen.");
        }
        public void WriteReject(WeatherSample sample, string reason)
        {
            fileManager.AppendReject(reason,
                $"{sample.Date},{sample.Temperature},{sample.Pressure}");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[REJECTED] {sample.Date} - {reason}");
            Console.ResetColor();
        }
        public WeatherServiceResponse CloseSession()
        {
            fileManager.FinalizeSession();
            fileManager.Dispose();
            fileManager = null;

            analytics.RaiseTransferCompleted(currentSessionId);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[ZAVRSEN PRENOS] SessionId: {currentSessionId}");
            Console.ResetColor();

            return new WeatherServiceResponse(
                ResponseStatus.ACK,
                SessionStatus.COMPLETED,
                $"Sesija {currentSessionId} uspesno zavrsena.");
        }
    }
}
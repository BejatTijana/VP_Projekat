using System;
using System.IO;
using Common;
using Service.Events;

namespace Service
{
    public class SessionManager : IDisposable
    {
        private StreamWriter csvWriter;
        private StreamWriter rejectsWriter;
        private bool disposed = false;
        private string sessionFile;
        private string rejectsFile;

        public delegate void TransferHandler(object sender, TransferEventArgs e);
        public delegate void SampleHandler(object sender, SampleReceivedEventArgs e);

        public event TransferHandler OnTransferStarted;
        public event TransferHandler OnTransferCompleted;
        public event SampleHandler OnSampleReceived;

        public void StartSession(SessionMeta meta)
        {
            string folder = "measurements";
            Directory.CreateDirectory(folder);
            sessionFile = Path.Combine(folder, $"measurements_session_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            rejectsFile = Path.Combine(folder, "rejects.csv");

            csvWriter = new StreamWriter(sessionFile, append: true);
            rejectsWriter = new StreamWriter(rejectsFile, append: true);

            csvWriter.WriteLine("Date,T,Pressure,Tpot,Tdew,VPmax,VPdef,VPact");

            OnTransferStarted?.Invoke(this, new TransferEventArgs(meta.StationName));
            Console.WriteLine("[Server] Sesija zapoceta. Prenos u toku...");
        }

        public void SaveSample(WeatherSample sample)
        {
            string line = $"{sample.Date},{sample.T},{sample.Pressure},{sample.Tpot}," +
                          $"{sample.Tdew},{sample.VPmax},{sample.VPdef},{sample.VPact}";
            csvWriter.WriteLine(line);
            OnSampleReceived?.Invoke(this, new SampleReceivedEventArgs(sample));
        }

        public void SaveReject(WeatherSample sample, string reason)
        {
            rejectsWriter.WriteLine($"{sample.Date} - ODBIJENO: {reason}");
        }

        public void EndSession(string stationName)
        {
            OnTransferCompleted?.Invoke(this, new TransferEventArgs(stationName));
            Console.WriteLine("[Server] Prenos zavrsen.");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    csvWriter?.Dispose();
                    rejectsWriter?.Dispose();
                }
                disposed = true;
            }
        }

        ~SessionManager() { Dispose(false); }
    }
}
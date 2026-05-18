using System;
using System.IO;
using System.Configuration;
using Common;

namespace Service
{
    class FileManager : IDisposable
    {
        private StreamWriter measurementsWriter;
        private StreamWriter rejectsWriter;
        private bool disposed = false;
        private string sessionPath;
        private string rejectsPath;

        public FileManager()
        {
            string basePath = ConfigurationManager.AppSettings["path"];

            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            sessionPath = Path.Combine(basePath, "measurements_session.csv");
            rejectsPath = Path.Combine(basePath, "rejects.csv");
        }

        ~FileManager()
        {
            Dispose(false);
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
                    if (measurementsWriter != null)
                    {
                        measurementsWriter.Flush();
                        measurementsWriter.Close();
                        measurementsWriter.Dispose();
                        measurementsWriter = null;
                    }
                    if (rejectsWriter != null)
                    {
                        rejectsWriter.Flush();
                        rejectsWriter.Close();
                        rejectsWriter.Dispose();
                        rejectsWriter = null;
                    }
                }
                disposed = true;
            }
        }

        public void InitializeSession()
        {
            measurementsWriter = new StreamWriter(
                new FileStream(sessionPath, FileMode.Create, FileAccess.Write));

            measurementsWriter.WriteLine(
                "Date,Temperature,Pressure,Tpot,Tdew,VPmax,VPdef,VPact");

            rejectsWriter = new StreamWriter(
                new FileStream(rejectsPath, FileMode.Create, FileAccess.Write));

            rejectsWriter.WriteLine("Reason,RawData");

            Console.WriteLine($"Sesija inicijalizovana. Fajl: {sessionPath}");
        }

        public void AppendSample(WeatherSample sample)
        {
            if (measurementsWriter == null)
            {
                throw new InvalidOperationException(
                    "FileManager nije inicijalizovan. Pozovite InitializeSession().");
            }

            measurementsWriter.WriteLine(
                $"{sample.Date},{sample.Temperature},{sample.Pressure}," +
                $"{sample.Tpot},{sample.Tdew},{sample.VPmax}," +
                $"{sample.VPdef},{sample.VPact}");

            measurementsWriter.Flush();
        }

        public void AppendReject(string reason, string rawData)
        {
            if (rejectsWriter == null)
            {
                throw new InvalidOperationException(
                    "FileManager nije inicijalizovan. Pozovite InitializeSession().");
            }

            rejectsWriter.WriteLine($"{reason},{rawData}");
            rejectsWriter.Flush();
        }

        public void FinalizeSession()
        {
            if (measurementsWriter != null)
            {
                measurementsWriter.Flush();
                Console.WriteLine($"Snimanje zavrseno. Fajl: {sessionPath}");
            }
        }
    }
}
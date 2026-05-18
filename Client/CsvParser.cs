using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using Common;

namespace Client
{
    class CsvReader : IDisposable
    {
        private StreamReader reader;
        private StreamWriter logWriter;
        private bool disposed = false;
        private string csvPath;
        private int maxRows;
        private string logPath;

        public CsvReader()
        {
            csvPath = ConfigurationManager.AppSettings["csvPath"];
            maxRows = int.Parse(ConfigurationManager.AppSettings["maxRows"]);

            string logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }
            logPath = Path.Combine(logFolder, "client_log.txt");
        }

        ~CsvReader()
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
                    if (reader != null)
                    {
                        reader.Close();
                        reader.Dispose();
                        reader = null;
                    }
                    if (logWriter != null)
                    {
                        logWriter.Flush();
                        logWriter.Close();
                        logWriter.Dispose();
                        logWriter = null;
                    }
                }
                disposed = true;
            }
        }

        public List<WeatherSample> ReadSamples()
        {
            List<WeatherSample> samples = new List<WeatherSample>();

            if (!File.Exists(csvPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR: CSV fajl nije pronadjen na putanji: {csvPath}");
                Console.ResetColor();
                return samples;
            }

            logWriter = new StreamWriter(
                new FileStream(logPath, FileMode.Create, FileAccess.Write));
            logWriter.WriteLine("Timestamp,Reason,RawData");

            reader = new StreamReader(
                new FileStream(csvPath, FileMode.Open, FileAccess.Read));

            
            string header = reader.ReadLine();

            int rowNumber = 0;
            int validCount = 0;

            while (!reader.EndOfStream && validCount < maxRows)
            {
                rowNumber++;
                string rawLine = reader.ReadLine();

                if (string.IsNullOrWhiteSpace(rawLine))
                {
                    LogReject(rowNumber, "Prazan red", rawLine);
                    continue;
                }

                WeatherSample sample = TryParseSample(rawLine, rowNumber);
                if (sample != null)
                {
                    samples.Add(sample);
                    validCount++;
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Ucitano {validCount} validnih uzoraka od {rowNumber} redova.");
            Console.ResetColor();
            Console.WriteLine($"Log nevalidnih redova: {logPath}");

            return samples;
        }

        private WeatherSample TryParseSample(string rawLine, int rowNumber)
        {
            try
            {
                string[] parts = rawLine.Split(',');

                
                if (parts.Length < 9)
                {
                    LogReject(rowNumber, "Nedovoljan broj kolona", rawLine);
                    return null;
                }

                DateTime date = DateTime.Parse(parts[0].Trim(),
                    CultureInfo.InvariantCulture);
                double pressure = double.Parse(parts[1].Trim(),
                    CultureInfo.InvariantCulture);
                double temperature = double.Parse(parts[2].Trim(),
                    CultureInfo.InvariantCulture);
                double tpot = double.Parse(parts[3].Trim(),
                    CultureInfo.InvariantCulture);
                double tdew = double.Parse(parts[4].Trim(),
                    CultureInfo.InvariantCulture);
                double vpMax = double.Parse(parts[6].Trim(),
                    CultureInfo.InvariantCulture);
                double vpAct = double.Parse(parts[7].Trim(),
                    CultureInfo.InvariantCulture);
                double vpDef = double.Parse(parts[8].Trim(),
                    CultureInfo.InvariantCulture);

                return new WeatherSample(temperature, pressure, tpot,
                    tdew, vpMax, vpDef, vpAct, date);
            }
            catch (Exception e)
            {
                LogReject(rowNumber, e.Message, rawLine);
                return null;
            }
        }

        private void LogReject(int rowNumber, string reason, string rawData)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[REJECTED] Red {rowNumber}: {reason}");
            Console.ResetColor();
            if (logWriter != null)
            {
                logWriter.WriteLine($"{DateTime.Now},{reason},Row {rowNumber}: {rawData}");
                logWriter.Flush();
            }
        }
    }
}
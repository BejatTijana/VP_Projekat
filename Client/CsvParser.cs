using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Common;

namespace Client
{
    public class CsvParser : IDisposable
    {
        private StreamReader reader;
        private bool disposed = false;
        private List<string> invalidRows = new List<string>();

        public CsvParser(string path)
        {
            reader = new StreamReader(path);
        }

        public List<WeatherSample> ParseFirst107Rows()
        {
            List<WeatherSample> samples = new List<WeatherSample>();
            string header = reader.ReadLine();
            int rowIndex = 0;

            while (!reader.EndOfStream && rowIndex < 107)
            {
                string line = reader.ReadLine();
                rowIndex++;
                WeatherSample sample = ParseLine(line, rowIndex);
                if (sample != null)
                    samples.Add(sample);
            }

            WriteInvalidLog();
            return samples;
        }

        private WeatherSample ParseLine(string line, int rowIndex)
        {
            try
            {
                string[] parts = line.Split(',');
                return new WeatherSample
                {
                    Date = parts[0].Trim(),
                    Pressure = double.Parse(parts[1].Trim(), CultureInfo.InvariantCulture),
                    T = double.Parse(parts[2].Trim(), CultureInfo.InvariantCulture),
                    Tpot = double.Parse(parts[3].Trim(), CultureInfo.InvariantCulture),
                    Tdew = double.Parse(parts[4].Trim(), CultureInfo.InvariantCulture),
                    VPmax = double.Parse(parts[6].Trim(), CultureInfo.InvariantCulture),
                    VPact = double.Parse(parts[7].Trim(), CultureInfo.InvariantCulture),
                    VPdef = double.Parse(parts[8].Trim(), CultureInfo.InvariantCulture),
                };
            }
            catch
            {
                invalidRows.Add($"Red {rowIndex}: {line}");
                return null;
            }
        }

        private void WriteInvalidLog()
        {
            if (invalidRows.Count > 0)
            {
                File.WriteAllLines("invalid_rows.log", invalidRows);
                Console.WriteLine($"[Client] {invalidRows.Count} nevalidnih redova zapisano u invalid_rows.log");
            }
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
                if (disposing) reader?.Dispose();
                disposed = true;
            }
        }

        ~CsvParser() { Dispose(false); }
    }
}
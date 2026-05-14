using System;
using System.Configuration;
using Common;
using Service.Events;

namespace Service
{
    public class WeatherAnalyzer
    {
        private double P_threshold;
        private double VPact_threshold;
        private double VPdef_threshold;

        private double pressureSum = 0;
        private int pressureCount = 0;
        private WeatherSample previous = null;

        public delegate void WarningHandler(object sender, WarningEventArgs e);
        public event WarningHandler OnWarningRaised;

        public WeatherAnalyzer()
        {
            P_threshold = double.Parse(ConfigurationManager.AppSettings["P_threshold"]);
            VPact_threshold = double.Parse(ConfigurationManager.AppSettings["VPact_threshold"]);
            VPdef_threshold = double.Parse(ConfigurationManager.AppSettings["VPdef_threshold"]);
        }

        public void Analyze(WeatherSample current)
        {
            if (previous == null)
            {
                previous = current;
                pressureSum += current.Pressure;
                pressureCount++;
                return;
            }

            CheckPressureSpike(current);
            CheckVPActSpike(current);
            CheckVPDefSpike(current);

            pressureSum += current.Pressure;
            pressureCount++;
            previous = current;
        }

        private void CheckPressureSpike(WeatherSample current)
        {
            double deltaP = current.Pressure - previous.Pressure;
            if (Math.Abs(deltaP) > P_threshold)
            {
                string direction = deltaP > 0 ? "iznad" : "ispod";
                OnWarningRaised?.Invoke(this, new WarningEventArgs("PressureSpike", direction, current.Pressure));
            }

            double mean = pressureSum / pressureCount;
            if (current.Pressure < 0.75 * mean || current.Pressure > 1.25 * mean)
            {
                string direction = current.Pressure > mean ? "iznad" : "ispod";
                OnWarningRaised?.Invoke(this, new WarningEventArgs("OutOfBandWarning (Pressure)", direction, current.Pressure));
            }
        }

        private void CheckVPActSpike(WeatherSample current)
        {
            double delta = current.VPact - previous.VPact;
            if (Math.Abs(delta) > VPact_threshold)
            {
                string direction = delta > 0 ? "iznad" : "ispod";
                OnWarningRaised?.Invoke(this, new WarningEventArgs("VPActSpike", direction, current.VPact));
            }
        }

        private void CheckVPDefSpike(WeatherSample current)
        {
            double delta = current.VPdef - previous.VPdef;
            if (Math.Abs(delta) > VPdef_threshold)
            {
                string direction = delta > 0 ? "iznad" : "ispod";
                OnWarningRaised?.Invoke(this, new WarningEventArgs("VPDefSpike", direction, current.VPdef));
            }
        }
    }
}
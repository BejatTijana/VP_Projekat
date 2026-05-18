using System;
using System.Configuration;
using Common;

namespace Service
{
    public class WeatherAnalytics
    {

        public delegate void TransferEventHandler(object sender, TransferEventArgs e);
        public delegate void SampleEventHandler(object sender, SampleEventArgs e);
        public delegate void WarningEventHandler(object sender, WarningEventArgs e);

        public event TransferEventHandler OnTransferStarted;
        public event TransferEventHandler OnTransferCompleted;
        public event SampleEventHandler OnSampleReceived;
        public event WarningEventHandler OnWarningRaised;
        public event WarningEventHandler OnPressureSpike;
        public event WarningEventHandler OnVPActSpike;
        public event WarningEventHandler OnVPDefSpike;
        public event WarningEventHandler OnOutOfBandWarning;


        double pThreshold;
        double vpActThreshold;
        double vpDefThreshold;


        double previousPressure;
        double previousVPact;
        double previousVPdef;
        bool isFirstSample;


        double pressureSum;
        int sampleCount;

        public WeatherAnalytics()
        {
            pThreshold = double.Parse(
                ConfigurationManager.AppSettings["P_threshold"],
                System.Globalization.CultureInfo.InvariantCulture);
            vpActThreshold = double.Parse(
                ConfigurationManager.AppSettings["VPact_threshold"],
                System.Globalization.CultureInfo.InvariantCulture);
            vpDefThreshold = double.Parse(
                ConfigurationManager.AppSettings["VPdef_threshold"],
                System.Globalization.CultureInfo.InvariantCulture);
            isFirstSample = true;
            pressureSum = 0;
            sampleCount = 0;
        }

        public void RaiseTransferStarted(string sessionId)
        {
            if (OnTransferStarted != null)
            {
                OnTransferStarted(this, new TransferEventArgs(sessionId,
                    $"Prenos podataka zapocet. SessionId: {sessionId}"));
            }
        }

        public void RaiseTransferCompleted(string sessionId)
        {
            if (OnTransferCompleted != null)
            {
                OnTransferCompleted(this, new TransferEventArgs(sessionId,
                    $"Prenos podataka zavrsen. SessionId: {sessionId}"));
            }
        }

        public void RaiseSampleReceived(WeatherSample sample)
        {
            if (OnSampleReceived != null)
            {
                OnSampleReceived(this, new SampleEventArgs(sample,
                    $"Uzorak primljen: {sample.Date}"));
            }
        }

        public void AnalyzeSample(WeatherSample sample)
        {

            pressureSum += sample.Pressure;
            sampleCount++;
            double pressureMean = pressureSum / sampleCount;

            if (!isFirstSample)
            {
                CheckPressureSpike(sample, pressureMean);
                CheckVPActSpike(sample);
                CheckVPDefSpike(sample);
            }


            previousPressure = sample.Pressure;
            previousVPact = sample.VPact;
            previousVPdef = sample.VPdef;
            isFirstSample = false;
        }

        private void CheckPressureSpike(WeatherSample sample, double pressureMean)
        {
            double deltaP = sample.Pressure - previousPressure;

            if (Math.Abs(deltaP) > pThreshold)
            {
                string direction = deltaP > 0 ? "iznad ocekivanog" : "ispod ocekivanog";
                WarningEventArgs args = new WarningEventArgs(
                    "Pressure", sample.Pressure, previousPressure,
                    deltaP, direction,
                    $"PRESSURE SPIKE! Delta: {deltaP:F2} hPa | Smer: {direction}");

                if (OnPressureSpike != null)
                {
                    OnPressureSpike(this, args);
                }
                if (OnWarningRaised != null)
                {
                    OnWarningRaised(this, args);
                }
            }


            if (sample.Pressure < 0.75 * pressureMean ||
                sample.Pressure > 1.25 * pressureMean)
            {
                string direction = sample.Pressure > pressureMean ?
                    "iznad ocekivane vrijednosti" : "ispod ocekivane vrijednosti";
                WarningEventArgs args = new WarningEventArgs(
                    "Pressure", sample.Pressure, pressureMean,
                    sample.Pressure - pressureMean, direction,
                    $"OUT OF BAND! Pritisak: {sample.Pressure:F2} | " +
                    $"Prosek: {pressureMean:F2} | Smer: {direction}");

                if (OnOutOfBandWarning != null)
                {
                    OnOutOfBandWarning(this, args);
                }
                if (OnWarningRaised != null)
                {
                    OnWarningRaised(this, args);
                }
            }
        }

        private void CheckVPActSpike(WeatherSample sample)
        {
            double deltaVPact = sample.VPact - previousVPact;

            if (Math.Abs(deltaVPact) > vpActThreshold)
            {
                string direction = deltaVPact > 0 ? "iznad ocekivanog" : "ispod ocekivanog";
                WarningEventArgs args = new WarningEventArgs(
                    "VPact", sample.VPact, previousVPact,
                    deltaVPact, direction,
                    $"VPACT SPIKE! Delta: {deltaVPact:F2} hPa | Smer: {direction}");

                if (OnVPActSpike != null)
                {
                    OnVPActSpike(this, args);
                }
                if (OnWarningRaised != null)
                {
                    OnWarningRaised(this, args);
                }
            }
        }

        private void CheckVPDefSpike(WeatherSample sample)
        {
            double deltaVPdef = sample.VPdef - previousVPdef;

            if (Math.Abs(deltaVPdef) > vpDefThreshold)
            {
                string direction = deltaVPdef > 0 ? "iznad ocekivanog" : "ispod ocekivanog";
                WarningEventArgs args = new WarningEventArgs(
                    "VPdef", sample.VPdef, previousVPdef,
                    deltaVPdef, direction,
                    $"VPDEF SPIKE! Delta: {deltaVPdef:F2} hPa | Smer: {direction}");

                if (OnVPDefSpike != null)
                {
                    OnVPDefSpike(this, args);
                }
                if (OnWarningRaised != null)
                {
                    OnWarningRaised(this, args);
                }
            }
        }

        public void Reset()
        {
            previousPressure = 0;
            previousVPact = 0;
            previousVPdef = 0;
            isFirstSample = true;
            pressureSum = 0;
            sampleCount = 0;
        }
    }
}
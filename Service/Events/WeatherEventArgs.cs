using System;
using Common;

namespace Service.Events
{
    public class SampleReceivedEventArgs : EventArgs
    {
        public WeatherSample Sample { get; }
        public SampleReceivedEventArgs(WeatherSample sample) { Sample = sample; }
    }

    public class WarningEventArgs : EventArgs
    {
        public string ParameterName { get; }
        public string Direction { get; }
        public double Value { get; }
        public WarningEventArgs(string paramName, string direction, double value)
        {
            ParameterName = paramName;
            Direction = direction;
            Value = value;
        }
    }

    public class TransferEventArgs : EventArgs
    {
        public string SessionName { get; }
        public TransferEventArgs(string sessionName) { SessionName = sessionName; }
    }
}
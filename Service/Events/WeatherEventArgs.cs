using System;
using Common;

namespace Service
{
    public class TransferEventArgs : EventArgs
    {
        string sessionId;
        string message;

        public TransferEventArgs(string sessionId, string message)
        {
            this.sessionId = sessionId;
            this.message = message;
        }

        public string SessionId { get => sessionId; }
        public string Message { get => message; }
    }

    public class SampleEventArgs : EventArgs
    {
        WeatherSample sample;
        string message;

        public SampleEventArgs(WeatherSample sample, string message)
        {
            this.sample = sample;
            this.message = message;
        }

        public WeatherSample Sample { get => sample; }
        public string Message { get => message; }
    }

    public class WarningEventArgs : EventArgs
    {
        string parameterName;
        double currentValue;
        double previousValue;
        double delta;
        string direction;
        string message;

        public WarningEventArgs(string parameterName, double currentValue,
                                double previousValue, double delta,
                                string direction, string message)
        {
            this.parameterName = parameterName;
            this.currentValue = currentValue;
            this.previousValue = previousValue;
            this.delta = delta;
            this.direction = direction;
            this.message = message;
        }

        public string ParameterName { get => parameterName; }
        public double CurrentValue { get => currentValue; }
        public double PreviousValue { get => previousValue; }
        public double Delta { get => delta; }
        public string Direction { get => direction; }
        public string Message { get => message; }
    }
}
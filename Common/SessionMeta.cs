using System;
using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public class SessionMeta
    {
        string sessionId;
        DateTime startTime;
        int totalSamples;

        public SessionMeta(string sessionId, DateTime startTime, int totalSamples)
        {
            this.sessionId = sessionId;
            this.startTime = startTime;
            this.totalSamples = totalSamples;
        }

        [DataMember]
        public string SessionId { get => sessionId; set => sessionId = value; }

        [DataMember]
        public DateTime StartTime { get => startTime; set => startTime = value; }

        [DataMember]
        public int TotalSamples { get => totalSamples; set => totalSamples = value; }

        public override string ToString()
        {
            return $"SessionId: {sessionId} | StartTime: {startTime} | " +
                   $"TotalSamples: {totalSamples}";
        }
    }
}
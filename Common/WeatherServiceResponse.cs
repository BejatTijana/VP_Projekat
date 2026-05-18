using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public class WeatherServiceResponse
    {
        ResponseStatus responseStatus;
        SessionStatus sessionStatus;
        string message;

        public WeatherServiceResponse(ResponseStatus responseStatus,
                                      SessionStatus sessionStatus,
                                      string message)
        {
            this.responseStatus = responseStatus;
            this.sessionStatus = sessionStatus;
            this.message = message;
        }

        [DataMember]
        public ResponseStatus ResponseStatus
        {
            get => responseStatus;
            set => responseStatus = value;
        }

        [DataMember]
        public SessionStatus SessionStatus
        {
            get => sessionStatus;
            set => sessionStatus = value;
        }

        [DataMember]
        public string Message { get => message; set => message = value; }

        public override string ToString()
        {
            return $"[{responseStatus}] [{sessionStatus}] - {message}";
        }
    }
}
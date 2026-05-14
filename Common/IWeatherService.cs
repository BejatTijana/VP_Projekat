using System.ServiceModel;

namespace Common
{
    [ServiceContract]
    public interface IWeatherService
    {
        [OperationContract]
        [FaultContract(typeof(DataFormatFault))]
        string StartSession(SessionMeta meta);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        string PushSample(WeatherSample sample);

        [OperationContract]
        string EndSession();
    }
}
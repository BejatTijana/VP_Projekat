using System.ServiceModel;

namespace Common
{
    [ServiceContract]
    public interface IWeatherService
    {
        [OperationContract]
        [FaultContract(typeof(DataFormatFault))]
        WeatherServiceResponse StartSession(SessionMeta meta);

        [OperationContract]
        [FaultContract(typeof(DataFormatFault))]
        [FaultContract(typeof(ValidationFault))]
        WeatherServiceResponse PushSample(WeatherSample sample);

        [OperationContract]
        WeatherServiceResponse EndSession();
    }
}
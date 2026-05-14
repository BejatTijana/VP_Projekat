using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public class WeatherSample
    {
        [DataMember] public double T { get; set; }
        [DataMember] public double Pressure { get; set; }
        [DataMember] public double Tpot { get; set; }
        [DataMember] public double Tdew { get; set; }
        [DataMember] public double VPmax { get; set; }
        [DataMember] public double VPdef { get; set; }
        [DataMember] public double VPact { get; set; }
        [DataMember] public string Date { get; set; }
    }
}
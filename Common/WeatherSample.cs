using System;
using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public class WeatherSample
    {
        double temperature;
        double pressure;
        double tpot;
        double tdew;
        double vpMax;
        double vpDef;
        double vpAct;
        DateTime date;

        public WeatherSample(double temperature, double pressure, double tpot,
                             double tdew, double vpMax, double vpDef,
                             double vpAct, DateTime date)
        {
            this.temperature = temperature;
            this.pressure = pressure;
            this.tpot = tpot;
            this.tdew = tdew;
            this.vpMax = vpMax;
            this.vpDef = vpDef;
            this.vpAct = vpAct;
            this.date = date;
        }

        [DataMember]
        public double Temperature { get => temperature; set => temperature = value; }

        [DataMember]
        public double Pressure { get => pressure; set => pressure = value; }

        [DataMember]
        public double Tpot { get => tpot; set => tpot = value; }

        [DataMember]
        public double Tdew { get => tdew; set => tdew = value; }

        [DataMember]
        public double VPmax { get => vpMax; set => vpMax = value; }

        [DataMember]
        public double VPdef { get => vpDef; set => vpDef = value; }

        [DataMember]
        public double VPact { get => vpAct; set => vpAct = value; }

        [DataMember]
        public DateTime Date { get => date; set => date = value; }

        public override string ToString()
        {
            return $"Date: {date} | T: {temperature} | P: {pressure} | " +
                   $"Tpot: {tpot} | Tdew: {tdew} | VPmax: {vpMax} | " +
                   $"VPdef: {vpDef} | VPact: {vpAct}";
        }
    }
}
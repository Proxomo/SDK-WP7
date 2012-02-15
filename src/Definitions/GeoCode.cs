using System;
using System.Runtime.Serialization;

namespace Proxomo
{
    [DataContract(Namespace = "")]
    public partial class GeoCode
    {
        [DataMember(Name = "Address")]
        public string Address { get; set; }

        [DataMember(Name = "City")]
        public string City { get; set; }

        [DataMember(Name = "CountryCode")]
        public string CountryCode { get; set; }

        [DataMember(Name = "CountryName")]
        public string CountryName { get; set; }

        [DataMember(Name = "DSTOffset")]
        public string DSTOffset { get; set; }

        [DataMember(Name = "GMTOffset")]
        public string GMTOffset { get; set; }

        [DataMember(Name = "Latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "Longitude")]
        public double Longitude { get; set; }

        [DataMember(Name = "Precision")]
        public string Precision { get; set; }

        [DataMember(Name = "RawOffset")]
        public string RawOffset { get; set; }

        [DataMember(Name = "Score")]
        public double Score { get; set; }

        [DataMember(Name = "State")]
        public string State { get; set; }

        [DataMember(Name = "TimeZoneName")]
        public string TimeZoneName { get; set; }

        [DataMember(Name = "Zip")]
        public string Zip { get; set; }
    }
}
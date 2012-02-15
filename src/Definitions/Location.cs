using System.Runtime.Serialization;

namespace Proxomo
{
    [DataContract(Namespace = "")]
    public partial class Location
    {
        [DataMember(Name = "ID")]
        public string ID { get; set; }

        [DataMember(Name = "Name")]
        public string Name { get; set; }

        [DataMember(Name = "Category")]
        public Category Category { get; set; }

        [DataMember(Name = "Latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "Longitude")]
        public double Longitude { get; set; }

        [DataMember(Name = "Address1")]
        public string Address1 { get; set; }

        [DataMember(Name = "Address2")]
        public string Address2 { get; set; }

        [DataMember(Name = "City")]
        public string City { get; set; }

        [DataMember(Name = "State")]
        public string State { get; set; }

        [DataMember(Name = "Zip")]
        public string Zip { get; set; }

        [DataMember(Name = "CountryName")]
        public string CountryName { get; set; }

        [DataMember(Name = "CountryCode")]
        public string CountryCode { get; set; }

        [DataMember(Name = "LocationType")]
        public string LocationType { get; set; }

        [DataMember(Name = "TimeZoneName")]
        public string TimeZoneName { get; set; }

        [DataMember(Name = "LocationSecurity")]
        public LocationSecurity LocationSecurity { get; set; }

        [DataMember(Name = "PersonID")]
        public string PersonID { get; set; }

        [DataMember(Name = "UTCOffset")]
        public double UTCOffset { get; set; }
    }
}